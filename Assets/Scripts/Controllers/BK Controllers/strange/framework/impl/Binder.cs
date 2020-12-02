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
 * @class strange.framework.impl.Binder
 * 
 * Collection class for bindings.
 * 
 * Binders are a collection class (akin to ArrayList and Dictionary)
 * with the specific purpose of connecting lists of things that are
 * not necessarily related, but need some type of runtime association.
 * Binders are the core concept of the StrangeIoC framework, allowing
 * all the other functionality to exist and further functionality to
 * easily be created.
 * 
 * Think of each Binder as a collection of causes and effects, or actions
 * and reactions. If the Key action happens, it triggers the Value
 * action. So, for example, an Event may be the Key that triggers
 * instantiation of a particular class.
 *
 * <h2>Runtime Bindings</h2>
 * As of V1.0, Strange supports runtime bindings via JSON. This allows you to 
 * instruct Strange to create its bindings by using loaded data, for example via
 * a downloaded .json file, or a server response.
 *
 * binder.ConsumeBindings(stringOfLoadedJson);
 *
 * Below are examples for basic runtime
 * binding options for Binder. The complete set of JSON runtime bindings for all
 * officially supported binders can be found in The Big, Strange How-To:
 * (http://strangeioc.github.io/strangeioc/TheBigStrangeHowTo.html)
 *
 * <h3>Example: basic binding via JSON</h3>
 * The simplest possible binding is an array of objects. We bind "This" to "That"
 *
 * [
 * 	{
 * 		"Bind":"This",
 * 		"To":"That"
 * 	}
 * ]
 *
 * You can of course load as many bindings as you like in your array:
 *
 * [
 *	{
 *		"Bind":"This",
 *		"To":"That"
 *	},
 *	{
 *		"Bind":"Han",
 *		"To":"Leia"
 *	},
 *	{
 *		"Bind":"Greedo",
 *		"To":"Table"
 *	}
 *]
 *
 * You can name bindings as you would expect:
 *
 * [
 *	{
 *		"Bind":"Battle",
 *		"To":"Planet",
 *		"ToName":"Endor" 
 *	}
 * ]
 *
 * If you need more than a single item in a "Bind" or "To" statement, use an array.
 *
 * [
 *	{
 *		"Bind":["Luke", "Han", "Wedge", "Biggs"],
 *		"To":"Pilot"
 *	}
 * ]
 *
 * There is also an "Options" array for special behaviors required by
 * individual Binders. The core Binder supports "Weak".
 *
 * [
 *	{
 *		"Bind":"X-Wing",
 *		"To":"Ship",
 *		"Options":["Weak"]
 *	}
 * ]
 *
 * Other Binders support other Options. Here's a case from the InjectionBinder. Note
 * how Options can be either a string or an array.
 *
 * [
 *	{
 *		"Bind":"strange.unittests.ISimpleInterface",
 *		"To":"strange.unittests.SimpleInterfaceImplementer",
 *		"Options":"ToSingleton"
 *	}
 * ]
 */

using System;
using System.Collections.Generic;
using strange.framework.api;

namespace strange.framework.impl
{
    public class Binder : IBinder
    {
        /// A handler for resolving the nature of a binding during chained commands
        public delegate void BindingResolver(IBinding binding);

        /// Dictionary of all bindings
        /// Two-layer keys. First key to individual Binding keys,
        /// then to Binding names. (This wouldn't be required if
        /// Unity supported Tuple or HashSet.)
        protected Dictionary<object, Dictionary<object, IBinding>> bindings;

        private readonly Dictionary<object, Dictionary<IBinding, object>> conflicts;

        protected Binder()
        {
            bindings = new Dictionary<object, Dictionary<object, IBinding>>();
            conflicts = new Dictionary<object, Dictionary<IBinding, object>>();
        }

        public virtual IBinding Bind<T>()
        {
            return Bind(typeof(T));
        }

        public virtual IBinding Bind(object key)
        {
            IBinding binding;
            binding = GetRawBinding();
            binding.Bind(key);
            return binding;
        }

        public virtual IBinding GetBinding<T>()
        {
            return GetBinding(typeof(T), null);
        }

        public virtual IBinding GetBinding(object key)
        {
            return GetBinding(key, null);
        }

        public virtual IBinding GetBinding<T>(object name)
        {
            return GetBinding(typeof(T), name);
        }

        public virtual IBinding GetBinding(object key, object name)
        {
            if (conflicts.Count > 0)
            {
                var conflictSummary = "";
                var keys = conflicts.Keys;
                foreach (var k in keys)
                {
                    if (conflictSummary.Length > 0) conflictSummary += ", ";
                    conflictSummary += k.ToString();
                }

                throw new BinderException(
                    "Binder cannot fetch Bindings when the binder is in a conflicted state.\nConflicts: " +
                    conflictSummary, BinderExceptionType.CONFLICT_IN_BINDER);
            }

            if (bindings.ContainsKey(key))
            {
                var dict = bindings[key];
                name = name == null ? BindingConst.NULLOID : name;
                if (dict.ContainsKey(name)) return dict[name];
            }

            return null;
        }

        public virtual void Unbind<T>()
        {
            Unbind(typeof(T), null);
        }

        public virtual void Unbind(object key)
        {
            Unbind(key, null);
        }

        public virtual void Unbind<T>(object name)
        {
            Unbind(typeof(T), name);
        }

        public virtual void Unbind(object key, object name)
        {
            if (bindings.ContainsKey(key))
            {
                var dict = bindings[key];
                var bindingName = name == null ? BindingConst.NULLOID : name;
                if (dict.ContainsKey(bindingName)) dict.Remove(bindingName);
            }
        }

        public virtual void Unbind(IBinding binding)
        {
            if (binding == null) return;

            if (binding.keyConstraint == BindingConstraintType.ONE)
            {
                Unbind(binding.key, binding.name);
            }
            else
            {
                var keys = binding.key as object[];
                for (var i = 0; i < keys.Length; i++) Unbind(keys[i], binding.name);
            }
        }

        public virtual void RemoveValue(IBinding binding, object value)
        {
            if (binding == null || value == null) return;
            var key = binding.key;
            Dictionary<object, IBinding> dict;
            if (bindings.ContainsKey(key))
            {
                dict = bindings[key];
                if (dict.ContainsKey(binding.name))
                {
                    var useBinding = dict[binding.name];
                    useBinding.RemoveValue(value);

                    //If result is empty, clean it out
                    var values = useBinding.value as object[];
                    if (values == null || values.Length == 0) dict.Remove(useBinding.name);
                }
            }
        }

        public virtual void RemoveKey(IBinding binding, object key)
        {
            if (binding == null || key == null || bindings.ContainsKey(key) == false) return;
            var dict = bindings[key];
            if (dict.ContainsKey(binding.name))
            {
                var useBinding = dict[binding.name];
                useBinding.RemoveKey(key);
                var keys = useBinding.key as object[];
                if (keys != null && keys.Length == 0) dict.Remove(binding.name);
            }
        }

        public virtual void RemoveName(IBinding binding, object name)
        {
            if (binding == null || name == null) return;
            object key;
            if (binding.keyConstraint.Equals(BindingConstraintType.ONE))
            {
                key = binding.key;
            }
            else
            {
                var keys = binding.key as object[];
                key = keys[0];
            }

            var dict = bindings[key];
            if (dict.ContainsKey(name))
            {
                var useBinding = dict[name];
                useBinding.RemoveName(name);
            }
        }

        public virtual IBinding GetRawBinding()
        {
            return new Binding(resolver);
        }

        /**
		 * This method places individual Bindings into the bindings Dictionary
		 * as part of the resolving process. Note that while some Bindings
		 * may store multiple keys, each key takes a unique position in the
		 * bindings Dictionary.
		 * 
		 * Conflicts in the course of fluent binding are expected, but GetBinding
		 * will throw an error if there are any unresolved conflicts.
		 */
        public virtual void ResolveBinding(IBinding binding, object key)
        {
            //Check for existing conflicts
            if (conflicts.ContainsKey(key)) //does the current key have any conflicts?
            {
                var inConflict = conflicts[key];
                if (inConflict.ContainsKey(binding)) //Am I on the conflict list?
                {
                    var conflictName = inConflict[binding];
                    if (isConflictCleared(inConflict, binding)) //Am I now out of conflict?
                        clearConflict(key, conflictName, inConflict); //remove all from conflict list.
                    else
                        return; //still in conflict
                }
            }

            //Check for and assign new conflicts
            var bindingName = binding.name == null ? BindingConst.NULLOID : binding.name;
            Dictionary<object, IBinding> dict;
            if (bindings.ContainsKey(key))
            {
                dict = bindings[key];
                //Will my registration create a new conflict?
                if (dict.ContainsKey(bindingName))
                {
                    //If the existing binding is not this binding, and the existing binding is not weak
                    //If it IS weak, we will proceed normally and overwrite the binding in the dictionary
                    var existingBinding = dict[bindingName];
                    //if (existingBinding != binding && !existingBinding.isWeak)
                    //SDM2014-01-20: as part of cross-context implicit bindings fix, attempts by a weak binding to replace a non-weak binding are ignored instead of being 
                    if (existingBinding != binding)
                    {
                        if (!existingBinding.isWeak && !binding.isWeak)
                        {
                            //register both conflictees
                            registerNameConflict(key, binding, dict[bindingName]);
                            return;
                        }

                        if (existingBinding.isWeak && (!binding.isWeak || existingBinding.value == null ||
                                                       existingBinding.value is Type))
                            //SDM2014-01-20: (in relation to the cross-context implicit bindings fix)
                            // 1) if the previous binding is weak and the new binding is not weak, then the new binding replaces the previous;
                            // 2) but if the new binding is also weak, then it only replaces the previous weak binding if the previous binding
                            // has not already been instantiated:

                            //Remove the previous binding.
                            dict.Remove(bindingName);
                    }
                }
            }
            else
            {
                dict = new Dictionary<object, IBinding>();
                bindings[key] = dict;
            }

            //Remove nulloid bindings
            if (dict.ContainsKey(BindingConst.NULLOID) && dict[BindingConst.NULLOID].Equals(binding))
                dict.Remove(BindingConst.NULLOID);

            //Add (or override) our new binding!
            if (!dict.ContainsKey(bindingName)) dict.Add(bindingName, binding);
        }

        public virtual void OnRemove()
        {
        }

        /// The default handler for resolving bindings during chained commands
        protected virtual void resolver(IBinding binding)
        {
            var key = binding.key;
            if (binding.keyConstraint.Equals(BindingConstraintType.ONE))
            {
                ResolveBinding(binding, key);
            }
            else
            {
                var keys = key as object[];
                var aa = keys.Length;
                for (var a = 0; a < aa; a++) ResolveBinding(binding, keys[a]);
            }
        }

        /// Take note of bindings that are in conflict.
        /// This occurs routinely during fluent binding, but will spark an error if
        /// GetBinding is called while this Binder still has conflicts.
        private void registerNameConflict(object key, IBinding newBinding, IBinding existingBinding)
        {
            Dictionary<IBinding, object> dict;
            if (conflicts.ContainsKey(key) == false)
            {
                dict = new Dictionary<IBinding, object>();
                conflicts[key] = dict;
            }
            else
            {
                dict = conflicts[key];
            }

            dict[newBinding] = newBinding.name;
            dict[existingBinding] = newBinding.name;
        }

        /// Returns true if the provided binding and the binding in the dict are no longer conflicting
        private bool isConflictCleared(Dictionary<IBinding, object> dict, IBinding binding)
        {
            foreach (var kv in dict)
                if (kv.Key != binding && kv.Key.name == binding.name)
                    return false;
            return true;
        }

        private void clearConflict(object key, object name, Dictionary<IBinding, object> dict)
        {
            var removalList = new List<IBinding>();

            foreach (var kv in dict)
            {
                var v = kv.Value;
                if (v.Equals(name)) removalList.Add(kv.Key);
            }

            var aa = removalList.Count;
            for (var a = 0; a < aa; a++) dict.Remove(removalList[a]);
            if (dict.Count == 0) conflicts.Remove(key);
        }
    }
}