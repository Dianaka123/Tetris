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
 * @class strange.extensions.reflector.impl.ReflectionBinder
 * 
 * Uses System.Reflection to create `ReflectedClass` instances.
 * 
 * Reflection is a slow process. This binder isolates the calls to System.Reflector 
 * and caches the result, meaning that Reflection is performed only once per class.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.reflector.api;
using strange.framework.api;
using Binder = strange.framework.impl.Binder;

namespace strange.extensions.reflector.impl
{
    public class ReflectionBinder : Binder, IReflectionBinder
    {
        public ReflectedClass Get<T>()
        {
            return Get(typeof(T));
        }

        public ReflectedClass Get(Type type)
        {
            var binding = GetBinding(type);
            if (binding == null)
            {
                binding = GetRawBinding();

                var reflected = new ReflectedClass();
                reflected.Type = type;
                mapPreferredConstructor(reflected, type);
                mapSetters(reflected, type); //map setters before mapping methods
                mapMethods(reflected, type);

                binding.Bind(type).To(reflected);
            }

            return binding.value as ReflectedClass;
        }

        public override IBinding GetRawBinding()
        {
            var binding = base.GetRawBinding();
            binding.valueConstraint = BindingConstraintType.ONE;
            return binding;
        }

        //Look for a constructor in the order:
        //1. Only one (just return it, since it's our only option)
        //2. Tagged with [Construct] tag
        //3. The constructor with the fewest parameters
        private ConstructorInfo findPreferredConstructor(Type type)
        {
            var constructors = type.GetConstructors(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.InvokeMethod);

            if (constructors.Length == 1) return constructors[0];

            ConstructorInfo constructorInfo = default;
            for (var index = 0; index < constructors.Length; index++)
            {
                var constructor = constructors[index];
                var taggedConstructors = constructor.GetCustomAttributes(typeof(Construct), true);
                if (taggedConstructors.Length > 0)
                {
                    if (constructorInfo != null)
                        throw new InjectionException("Class has already Construct  " + type,
                            InjectionExceptionType.ILLIGAL_USAGE);

                    constructorInfo = constructor;
                }
            }

            return constructorInfo;
        }

        private void mapPreferredConstructor(ReflectedClass reflected, Type type)
        {
            var constructor = findPreferredConstructor(type);
            if (constructor == null)
                throw new ReflectionException(
                    "The reflector requires concrete classes.\nType " + type +
                    " has no constructor. Is it an interface?",
                    ReflectionExceptionType.CANNOT_REFLECT_INTERFACE);

            var parameters = constructor.GetParameters();

            var paramList = new Type[parameters.Length];
            var names = new object[parameters.Length];
            var i = 0;
            foreach (var param in parameters)
            {
                var paramType = param.ParameterType;
                paramList[i] = paramType;

                if (param.IsDefined(typeof(Name), true))
                {
                    var attributes = param.GetCustomAttributes(typeof(Name), false);
                    names[i] = ((Name) attributes[0]).name;
                }

                i++;
            }

            reflected.Constructor = constructor;
            reflected.ConstructorParameters = paramList;
            reflected.ConstructorParameterNames = names;
        }


        private void mapMethods(ReflectedClass reflected, Type type)
        {
            var methods = type.GetMethods(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.InvokeMethod);

            for (var index = 0; index < methods.Length; index++)
            {
                var method = methods[index];
                var hasAttribute = method.IsDefined(typeof(PostConstruct));
                if (hasAttribute)
                {
                    if (reflected.PostConstructor != null)
                        throw new InjectionException("Class has already Posconstruct " + type,
                            InjectionExceptionType.ILLIGAL_USAGE);

                    reflected.PostConstructor = method;
                    continue;
                }

                hasAttribute = method.IsDefined(typeof(Deconstruct));
                if (hasAttribute)
                {
                    if (reflected.DeConstructor != null)
                        throw new InjectionException("Class has already Deconstruct " + type,
                            InjectionExceptionType.ILLIGAL_USAGE);

                    reflected.DeConstructor = method;
                }
            }
        }

        private void mapSetters(ReflectedClass reflected, Type type)
        {
#if UNITY_EDITOR
            //only for [Inject] private properties todo set prebuild step
            /*var privateMembers = type.FindMembers(
                MemberTypes.Property,
                BindingFlags.FlattenHierarchy |
                BindingFlags.SetProperty |
                BindingFlags.NonPublic |
                BindingFlags.Instance,
                null, null);

            foreach (var member in privateMembers)
            {
                var injections = member.GetCustomAttributes(typeof(Inject), true);
                if (injections.Length > 0)
                {
                    throw new ReflectionException(
                        "The class " + type.Name + " has a non-public Injection setter " + member.Name + ". Make the setter public to allow injection.",
                        ReflectionExceptionType.CANNOT_INJECT_INTO_NONPUBLIC_SETTER);
                }
            }*/
#endif
            var members = type.FindMembers(
                MemberTypes.Property,
                BindingFlags.FlattenHierarchy |
                BindingFlags.SetProperty |
                BindingFlags.Public |
                BindingFlags.Instance,
                null, null);

            var pairs = new KeyValuePair<Type, PropertyInfo>[0];
            var names = new object[0];

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                if (member.IsDefined(typeof(Inject), true))
                {
                    var injections = member.GetCustomAttributes(typeof(Inject), true);
                    var attr = injections[0] as Inject;
                    var point = member as PropertyInfo;
                    var pointType = point.PropertyType;
                    var pair = new KeyValuePair<Type, PropertyInfo>(pointType, point);
                    pairs = AddKV(pair, pairs);

                    var bindingName = attr.name;
                    names = Add(bindingName, names);
                }
            }

            reflected.Setters = pairs;
            reflected.SetterNames = names;
        }

        /**
         * Add an item to a list
         */
        private object[] Add(object value, object[] list)
        {
            var tempList = list;
            var len = tempList.Length;
            list = new object[len + 1];
            tempList.CopyTo(list, 0);
            list[len] = value;
            return list;
        }

        /**
         * Add an item to a list
         */
        private KeyValuePair<Type, PropertyInfo>[] AddKV(KeyValuePair<Type, PropertyInfo> value,
            KeyValuePair<Type, PropertyInfo>[] list)
        {
            var tempList = list;
            var len = tempList.Length;
            list = new KeyValuePair<Type, PropertyInfo>[len + 1];
            tempList.CopyTo(list, 0);
            list[len] = value;
            return list;
        }
    }
}