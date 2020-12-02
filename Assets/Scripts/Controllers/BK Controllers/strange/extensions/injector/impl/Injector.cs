/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions.injector.impl.Injector
 * 
 * Supplies injection for all mapped dependencies. 
 * 
 * Extension satisfies injection dependencies. Works in conjuntion with 
 * (and therefore relies on) the Reflector.
 * 
 * Dependencies may be Constructor injections (all parameters will be satisfied),
 * or setter injections.
 * 
 * Classes utilizing this injector must be marked with the following metatags:
 * <ul>
 *  <li>[Inject] - Use this metatag on any setter you wish to have supplied by injection.</li>
 *  <li>[Construct] - Use this metatag on the specific Constructor you wish to inject into when using Constructor injection. If you omit this tag, the Constructor with the shortest list of dependencies will be selected automatically.</li>
 *  <li>[PostConstruct] - Use this metatag on any method(s) you wish to fire directly after dependencies are supplied</li>
 * </ul>
 * 
 * The Injection system is quite loud and specific where dependencies are unmapped,
 * throwing Exceptions to warn you. This is exceptionally useful in ensuring that
 * your app is well structured.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using strange.extensions.context.api;
using strange.extensions.injector.api;
using strange.extensions.reflector.api;
using strange.extensions.reflector.impl;
using UnityEngine;

namespace strange.extensions.injector.impl
{
    public class Injector : IInjector
    {
        private const int InfinityLimit = 10;
        private readonly List<DeconstructBucket> _deconstructed;
        private readonly List<IDisposable> _disposed;
        private readonly IInjectorFactory _factory;

        private Dictionary<IInjectionBinding, int> _infinityLock;

        public Injector()
        {
            _factory = new InjectorFactory();
            _disposed = new List<IDisposable>(1000);
            _deconstructed = new List<DeconstructBucket>(1000);
        }

        public IInjectionBinder binder { get; set; }
        public IReflectionBinder reflector { get; set; }

        public T Instantiate<T>(Type type)
        {
            var reflection = reflector.Get(type);

            failIf(
                reflection.Constructor == null, "Attempt to construction inject a null constructor",
                InjectionExceptionType.NULL_CONSTRUCTOR);

#if STRANGE_ANALYSIS
            T value = default;
            StrangeDebugger.Measure(
                () =>
                {
                    var args2 = GetArgs(reflection);
                    value = (T) reflection.Constructor.Invoke(args2);
                }, null, type.Name, "ConstructorInjection");
            return value;
#endif
            var args = GetArgs(reflection);
            return (T) reflection.Constructor.Invoke(args);
        }

        public object Instantiate(IInjectionBinding binding, bool tryInjectHere)
        {
            failIf(
                binder == null, "Attempt to instantiate from Injector without a Binder",
                InjectionExceptionType.NO_BINDER);
            failIf(
                _factory == null, "Attempt to inject into Injector without a Factory",
                InjectionExceptionType.NO_FACTORY);

            armorAgainstInfiniteLoops(binding);

            object retv = null;
            Type reflectionType = null;

            if (binding.value is Type)
            {
                reflectionType = binding.value as Type;
            }
            else if (binding.value == null)
            {
                var tl = binding.key as object[];
                reflectionType = tl[0] as Type;
                if (reflectionType.IsPrimitive || reflectionType == typeof(decimal) || reflectionType == typeof(string))
                    retv = binding.value;
            }
            else
            {
                retv = binding.value;
            }

            if (retv == null) //If we don't have an existing value, go ahead and create one.
            {
                var reflection = reflector.Get(reflectionType);

#if STRANGE_ANALYSIS
                strange.extensions.analysis.impl.StrangeDebugger.Measure(
                    () =>
                    {
                        var args = GetArgs(reflection);
                        retv = _factory.Get(binding, args);
                    }, null, reflectionType?.Name, "ConstructorInjection");
#else
                var args = GetArgs(reflection);
                retv = _factory.Get(binding, args);
#endif
                if (tryInjectHere) TryInject(binding, retv);
            }

            _infinityLock =
                null; //Clear our infinity lock so the next time we instantiate we don't consider this a circular dependency

            return retv;
        }

        public object TryInject(IInjectionBinding binding, object target)
        {
            //If the InjectorFactory returns null, just return it. Otherwise inject the retv if it needs it
            //This could happen if Activator.CreateInstance returns null
            if (target != null)
            {
                if (binding.toInject) target = Inject(target, binding.IsAvoidDestroy);

                if (binding.type == InjectionBindingType.SINGLETON || binding.type == InjectionBindingType.VALUE)
                    binding.ToInject(false);
            }

            return target;
        }

        public object Inject(object target, bool isAvoidDestroy = false)
        {
            failIf(binder == null, "Attempt to inject into Injector without a Binder",
                InjectionExceptionType.NO_BINDER);
            failIf(reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
            failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

            //Some things can't be injected into. Bail out.
            var t = target.GetType();
            if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string)) return target;

            ReflectedClass reflection = default;
#if !STRANGE_ANALYSIS
            reflection = reflector.Get(t);
#else
            StrangeDebugger.Measure(() => reflection = reflector.Get(t), null, target, "GetReflectedInfo");
#endif

            failIf(
                reflection == null, "Attempt to PostConstruct without a reflection",
                InjectionExceptionType.NULL_REFLECTION);

            if (reflection.Setters.Length > 0)
            {
#if !STRANGE_ANALYSIS
                performSetterInjection(target, reflection);
#else
                StrangeDebugger.Measure(() => performSetterInjection(target, reflection), null, target, "SetterInjection");
#endif
            }

            if (reflection.PostConstructor != null)
            {
#if !STRANGE_ANALYSIS
                postInject(target, reflection);
#else
                StrangeDebugger.Measure(() => postInject(target, reflection), null, target, "PostConstruct");
#endif
            }

            if (reflection.DeConstructor != null && !isAvoidDestroy)
                _deconstructed.Add(new DeconstructBucket {Target = target, Deconstruct = reflection.DeConstructor});

            //exclude context to avoid dead loop. Dispose on context must be called manually
            if (target is IDisposable && !(target is IContext) && !isAvoidDestroy) _disposed.Add((IDisposable) target);

            return target;
        }

        public void Uninject(object target)
        {
            failIf(binder == null, "Attempt to inject into Injector without a Binder",
                InjectionExceptionType.NO_BINDER);
            failIf(reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
            failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

            var t = target.GetType();
            if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string)) return;

            var reflection = reflector.Get(t);

            performUninjection(target, reflection);
        }

        void IInjector.Deconstruct()
        {
            var count = _deconstructed.Count;
            for (var index = 0; index < count; index++)
            {
                var bucket = _deconstructed[index];
                if (bucket.Deconstruct != null && bucket.Target != null)
                {
#if !STRANGE_ANALYSIS
                    try
                    {
                        bucket.Deconstruct.Invoke(bucket.Target, null);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
#else
                    StrangeDebugger.Measure(() => bucket.Deconstruct.Invoke(bucket.Target, null), null, bucket.Target, "DeconstructInfo");
#endif
                }
            }

            _deconstructed.Clear();
        }

        void IInjector.Dispose()
        {
            var count = _disposed.Count;
            for (var index = 0; index < count; index++)
            {
                var target = _disposed[index];
                if (target != null)
                {
#if !STRANGE_ANALYSIS
                    try
                    {
                        target.Dispose();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
#else
                    StrangeDebugger.Measure(() => target.Dispose(), null, target, "DisposeInfo");
#endif
                }
            }

            _disposed.Clear();
        }

        private object[] GetArgs(ReflectedClass reflection)
        {
            var parameterTypes = reflection.ConstructorParameters;
            var parameterNames = reflection.ConstructorParameterNames;

            var aa = parameterTypes.Length;
            var args = new object [aa];
            for (var a = 0; a < aa; a++)
                args[a] = getValueInjection(parameterTypes[a], parameterNames[a], reflection.Type, null);

            return args;
        }

        //https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
        //After injection, call any methods labelled with the [PostConstruct] tag
        private void postInject(object target, ReflectedClass reflection)
        {
            reflection.PostConstructor.Invoke(target, null);
        }

        private void performSetterInjection(object target, ReflectedClass reflection)
        {
            failIf(
                reflection.Setters.Length != reflection.SetterNames.Length,
                "Attempt to perform setter injection with mismatched names.\nThere must be exactly as many names as setters.",
                InjectionExceptionType.SETTER_NAME_MISMATCH);

            var aa = reflection.Setters.Length;
            for (var a = 0; a < aa; a++)
            {
                var pair = reflection.Setters[a];
                var value = getValueInjection(pair.Key, reflection.SetterNames[a], target, pair.Value);
                injectValueIntoPoint(value, target, pair.Value);
            }
        }

        private object getValueInjection(Type t,
            object name,
            object target,
            PropertyInfo propertyInfo)
        {
            IInjectionBinding suppliedBinding = null;
            if (target != null)
                suppliedBinding = binder.GetSupplier(
                    t, target is Type
                        ? target as Type
                        : target.GetType());

            var binding = suppliedBinding ?? binder.GetBinding(t, name);

            failIf(
                binding == null, "Attempt to Instantiate a null binding", InjectionExceptionType.NULL_BINDING, t,
                name, target, propertyInfo);

            if (binding.type == InjectionBindingType.VALUE)
            {
                if (!binding.toInject) return binding.value;

                var retv = Inject(binding.value, binding.IsAvoidDestroy);
                binding.ToInject(false);
                return retv;
            }

            if (binding.type == InjectionBindingType.SINGLETON)
            {
                if (binding.value is Type || binding.value == null) Instantiate(binding, true);

                return binding.value;
            }

            return Instantiate(binding, true);
        }

        //Inject the value into the target at the specified injection point
        private void injectValueIntoPoint(object value,
            object target,
            PropertyInfo point)
        {
            failIf(target == null, "Attempt to inject into a null target", InjectionExceptionType.NULL_TARGET);
            failIf(point == null, "Attempt to inject into a null point", InjectionExceptionType.NULL_INJECTION_POINT);
            failIf(value == null, "Attempt to inject null into a target object",
                InjectionExceptionType.NULL_VALUE_INJECTION);
#if NET_STANDARD_2_0 || NET_4_6
            point = point.DeclaringType.GetProperty(point.Name);
#endif
            point.SetValue(target, value, null);
        }

        //Note that uninjection can only clean publicly settable points
        private void performUninjection(object target, ReflectedClass reflection)
        {
            var aa = reflection.Setters.Length;
            for (var a = 0; a < aa; a++)
            {
                var pair = reflection.Setters[a];
                pair.Value.SetValue(target, null, null);
            }
        }

        private void armorAgainstInfiniteLoops(IInjectionBinding binding)
        {
            if (binding == null) return;

            if (_infinityLock == null) _infinityLock = new Dictionary<IInjectionBinding, int>();

            if (_infinityLock.ContainsKey(binding) == false) _infinityLock.Add(binding, 0);

            _infinityLock[binding] = _infinityLock[binding] + 1;
            if (_infinityLock[binding] > InfinityLimit)
            {
                var type = "Undefined";
                if (binding.value is Type toType) type = toType.Name;

                throw new InjectionException(
                    "There appears to be a circular dependency. Terminating loop. " + type,
                    InjectionExceptionType.CIRCULAR_DEPENDENCY);
            }
        }

        private void failIf(bool condition,
            string message,
            InjectionExceptionType type)
        {
            failIf(condition, message, type, null, null, null);
        }

        private void failIf(bool condition,
            string message,
            InjectionExceptionType type,
            Type t,
            object name)
        {
            failIf(condition, message, type, t, name, null);
        }

        private void failIf(
            bool condition,
            string message,
            InjectionExceptionType type,
            Type t,
            object name,
            object target,
            PropertyInfo propertyInfo)
        {
            if (condition)
            {
                if (propertyInfo != null) message += "\n\t\ttarget property: " + propertyInfo.Name;

                failIf(true, message, type, t, name, target);
            }
        }

        private void failIf(
            bool condition,
            string message,
            InjectionExceptionType type,
            Type t,
            object name,
            object target)
        {
            if (condition)
            {
                message += "\n\t\ttarget: " + target;
                message += "\n\t\ttype: " + t;
                message += "\n\t\tname: " + name;
                throw new InjectionException(message, type);
            }
        }

        private struct DeconstructBucket
        {
            public MethodInfo Deconstruct;
            public object Target;
        }
    }
}