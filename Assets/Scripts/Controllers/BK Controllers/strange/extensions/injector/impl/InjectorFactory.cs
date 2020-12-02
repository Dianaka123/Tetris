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
 * @class strange.extensions.injector.impl.InjectorFactory
 * 
 * The Factory that instantiates all instances.
 */

using System;
using strange.extensions.injector.api;

namespace strange.extensions.injector.impl
{
    public class InjectorFactory : IInjectorFactory
    {
        object IInjectorFactory.Get(IInjectionBinding binding, object[] args)
        {
            if (binding == null)
                throw new InjectionException("InjectorFactory cannot act on null binding",
                    InjectionExceptionType.NULL_BINDING);
            var type = binding.type;

            switch (type)
            {
                case InjectionBindingType.SINGLETON:
                    return singletonOf(binding, args);
                case InjectionBindingType.VALUE:
                    return valueOf(binding);
            }

            return instanceOf(binding, args);
        }

        /// Generate a Singleton instance
        private object singletonOf(IInjectionBinding binding, object[] args)
        {
            if (binding.value != null)
            {
                if (binding.value.GetType().IsInstanceOfType(typeof(Type)))
                {
                    var o = createFromValue(binding.value, args);
                    if (o == null)
                        return null;
                    binding.SetValue(o);
                }
            }
            else
            {
                binding.SetValue(generateImplicit((binding.key as object[])[0], args));
            }

            return binding.value;
        }

        private object generateImplicit(object key, object[] args)
        {
            var type = key as Type;
            if (!type.IsInterface && !type.IsAbstract) return createFromValue(key, args);
            throw new InjectionException(
                "InjectorFactory can't instantiate an Interface or Abstract Class. Class: " + key,
                InjectionExceptionType.NOT_INSTANTIABLE);
        }

        /// The binding already has a value. Simply return it.
        private object valueOf(IInjectionBinding binding)
        {
            return binding.value;
        }

        /// Generate a new instance
        private object instanceOf(IInjectionBinding binding, object[] args)
        {
            if (binding.value != null) return createFromValue(binding.value, args);
            var value = generateImplicit((binding.key as object[])[0], args);
            return createFromValue(value, args);
        }

        /// Call the Activator to attempt instantiation the given object
        private object createFromValue(object o, object[] args)
        {
            var value = o is Type ? o as Type : o.GetType();
            object retv = null;
            try
            {
                if (args == null || args.Length == 0)
                    retv = Activator.CreateInstance(value);
                else
                    retv = Activator.CreateInstance(value, args);
            }
            catch
            {
                //No-op
            }

            return retv;
        }
    }
}