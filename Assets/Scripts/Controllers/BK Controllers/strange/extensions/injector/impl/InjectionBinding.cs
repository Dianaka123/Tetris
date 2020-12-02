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
 * @class strange.extensions.injector.impl.InjectionBinding
 * 
 * The Binding for Injections.
 * 
 * @see strange.extensions.injector.api.IInjectionBinding
 */

using System;
using strange.extensions.injector.api;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.injector.impl
{
    public class InjectionBinding : Binding, IInjectionBinding
    {
        private readonly ISemiBinding supplyList = new SemiBinding();

        public InjectionBinding(Binder.BindingResolver resolver)
        {
            this.resolver = resolver;
            keyConstraint = BindingConstraintType.MANY;
            valueConstraint = BindingConstraintType.ONE;
            supplyList.constraint = BindingConstraintType.MANY;
        }

        public InjectionBindingType type { get; set; } = InjectionBindingType.DEFAULT;

        public bool toInject { get; private set; } = true;

        public IInjectionBinding ToInject(bool value)
        {
            toInject = value;
            return this;
        }

        public bool isCrossContext { get; private set; }

        public bool IsAvoidDestroy { get; private set; }

        public IInjectionBinding ToSingleton()
        {
            //If already a value, this mapping is redundant
            if (type == InjectionBindingType.VALUE)
                return this;

            type = InjectionBindingType.SINGLETON;
            if (resolver != null) resolver(this);
            return this;
        }

        public IInjectionBinding ToValue(object o)
        {
            type = InjectionBindingType.VALUE;
            SetValue(o);
            return this;
        }

        public IInjectionBinding SetValue(object o)
        {
            var objType = o.GetType();

            var keys = key as object[];
            var aa = keys.Length;
            //Check that value is legal for the provided keys
            for (var a = 0; a < aa; a++)
            {
                var aKey = keys[a];
                var keyType = aKey is Type ? aKey as Type : aKey.GetType();
                if (keyType.IsAssignableFrom(objType) == false && HasGenericAssignableFrom(keyType, objType) == false)
                    throw new InjectionException(
                        "Injection cannot bind a value that does not extend or implement the binding type.",
                        InjectionExceptionType.ILLEGAL_BINDING_VALUE);
            }

            To(o);
            return this;
        }

        public IInjectionBinding CrossContext()
        {
            isCrossContext = true;
            if (resolver != null) resolver(this);
            return this;
        }

        public IInjectionBinding AvoidDestroy()
        {
            IsAvoidDestroy = true;
            if (resolver != null) resolver(this);
            return this;
        }

        /// Promise this Binding to any instance of Type
        /// <T>
        public IInjectionBinding SupplyTo<T>()
        {
            return SupplyTo(typeof(T));
        }

        /// Promise this Binding to any instance of Type type
        public IInjectionBinding SupplyTo(Type type)
        {
            supplyList.Add(type);
            if (resolver != null) resolver(this);
            return this;
        }

        /// Remove the promise to supply this binding to Type
        /// <T>
        public IInjectionBinding Unsupply<T>()
        {
            return Unsupply(typeof(T));
        }

        /// Remove the promise to supply this binding to Type type
        public IInjectionBinding Unsupply(Type type)
        {
            supplyList.Remove(type);
            return this;
        }

        public object[] GetSupply()
        {
            return supplyList.value as object[];
        }

        public new IInjectionBinding Bind<T>()
        {
            return base.Bind<T>() as IInjectionBinding;
        }

        public new IInjectionBinding Bind(object key)
        {
            return base.Bind(key) as IInjectionBinding;
        }

        public new IInjectionBinding To<T>()
        {
            return base.To<T>() as IInjectionBinding;
        }

        public new IInjectionBinding To(object o)
        {
            return base.To(o) as IInjectionBinding;
        }

        public new IInjectionBinding ToName<T>()
        {
            return base.ToName<T>() as IInjectionBinding;
        }

        public new IInjectionBinding ToName(object o)
        {
            return base.ToName(o) as IInjectionBinding;
        }

        public new IInjectionBinding Named<T>()
        {
            return base.Named<T>() as IInjectionBinding;
        }

        public new IInjectionBinding Named(object o)
        {
            return base.Named(o) as IInjectionBinding;
        }

        protected bool HasGenericAssignableFrom(Type keyType, Type objType)
        {
            //FIXME: We need to figure out how to determine generic assignability
            if (keyType.IsGenericType == false)
                return false;

            return true;
        }

        protected bool IsGenericTypeAssignable(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsGenericTypeAssignable(baseType, genericType);
        }
    }
}