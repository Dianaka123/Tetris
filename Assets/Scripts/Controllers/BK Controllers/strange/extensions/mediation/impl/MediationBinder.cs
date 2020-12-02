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
 * @class strange.extensions.mediation.impl.MediationBinder
 * 
 * Binds Views to Mediators. This is the base class for all MediationBinders
 * that work with MonoBehaviours.
 * 
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.framework.api;
using strange.framework.impl;
using UnityEngine;

namespace strange.extensions.mediation.impl
{
    public sealed class MediationBinder : Binder, IMediationBinder
    {
        private readonly Dictionary<IView, Mediator>
            _viewMediatorLinksDictionary = new Dictionary<IView, Mediator>(100);

        [Inject] public IInjectionBinder injectionBinder { get; set; }

        public override IBinding GetRawBinding()
        {
            return new MediationBinding(resolver);
        }

        public void Trigger(MediationEvent evt, IView view)
        {
            var viewType = view.GetType();
            if (GetBinding(viewType) is IMediationBinding binding)
                switch (evt)
                {
                    case MediationEvent.AWAKE:
                        InjectViewAndChildren(view);
                        MapView(view, binding);
                        break;
                    case MediationEvent.DESTROYED:
                        UnmapView(view, binding);
                        break;
                    case MediationEvent.ENABLED:
                        EnableView(view, binding);
                        break;
                    case MediationEvent.DISABLED:
                        DisableView(view, binding);
                        break;
                }
            else if (evt == MediationEvent.AWAKE)
                //Even if not mapped, Views (and their children) have potential to be injected
                InjectViewAndChildren(view);
        }

        public new IMediationBinding Bind<T>()
        {
            return base.Bind<T>() as IMediationBinding;
        }

        public T GetMediator<T>(IView view) where T : Mediator
        {
            if (_viewMediatorLinksDictionary.TryGetValue(view, out var mediator))
            {
                if (mediator != null) return (T) mediator;
                Debug.LogError($"Mediator for {view} is null");
            }
            else
            {
                Debug.LogError($"Can't get mediator for view {view}");
            }

            return default;
        }

        /// Creates and registers one or more Mediators for a specific View instance.
        /// Takes a specific View instance and a binding and, if a binding is found for that type, creates and registers a Mediator.
        private void MapView(IView view, IMediationBinding binding)
        {
            var viewType = view.GetType();

            if (bindings.ContainsKey(viewType))
            {
                var values = binding.value as object[];
                if (values != null)
                {
                    var aa = values.Length;
                    for (var a = 0; a < aa; a++)
                    {
                        var mediatorType = values[a] as Type;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (mediatorType == viewType)
                            throw new MediationException(
                                viewType + "mapped to itself. The result would be a stack overflow.",
                                MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);

                        if (!typeof(Mediator).IsAssignableFrom(mediatorType))
                            throw new ArgumentException(
                                "Binding mediator with type: " + mediatorType +
                                " is not assignable from IMediator");
#endif
                        ApplyMediationToView(binding, view, mediatorType);

                        if (view.isActiveAndEnabled) EnableMediator(view, mediatorType);
                    }
                }
            }
        }

        /// Add a Mediator to a View. If the mediator is a "true" Mediator (i.e., it
        /// implements IMediator), perform PreRegister and OnRegister.
        private void ApplyMediationToView(IMediationBinding binding, IView view, Type mediatorType)
        {
            if (!_viewMediatorLinksDictionary.ContainsKey(view))
            {
                var mediator = injectionBinder.injector.Instantiate<Mediator>(mediatorType);
                if (mediator == null) throw new ArgumentException("Binding null mediator to view: " + view);

                mediator.BaseMediatedView = view;

                var typeToInject = binding.abstraction == null || binding.abstraction.Equals(BindingConst.NULLOID)
                    ? view.GetType()
                    : binding.abstraction as Type;

                injectionBinder.Bind(typeToInject).ToValue(view).ToInject(false);
                injectionBinder.injector.Inject(mediator);
                injectionBinder.Unbind(typeToInject);

                mediator.OnRegister();
                _viewMediatorLinksDictionary.Add(view, mediator);
            }
        }

        private void InjectViewAndChildren(IView view)
        {
            var views = GetViews(view);
            var aa = views.Length;
            //UnityEngine.Debug.LogError("InjectViewAndChildren: "+ views.Length+",view.GetType= "+ view.GetType());
            for (var a = aa - 1; a > -1; a--)
            {
                var iView = views[a];
                if (iView != null)
                {
                    if (iView.autoRegisterWithContext && iView.registeredWithContext) continue;

                    iView.registeredWithContext = true;
                    if (iView.Equals(view) == false) Trigger(MediationEvent.AWAKE, iView);
                }
            }

            injectionBinder.injector.Inject(view);
        }

        /// Removes a mediator when its view is destroyed
        private void UnmapView(IView view, IMediationBinding binding)
        {
            TriggerInBindings(view, binding, DestroyMediator);
        }

        /// Enables a mediator when its view is enabled
        private void EnableView(IView view, IMediationBinding binding)
        {
            TriggerInBindings(view, binding, EnableMediator);
        }

        /// Disables a mediator when its view is disabled
        private void DisableView(IView view, IMediationBinding binding)
        {
            TriggerInBindings(view, binding, DisableMediator);
        }

        /// Triggers given function in all mediators bound to given view
        private void TriggerInBindings(IView view, IMediationBinding binding, Func<IView, Type, object> method)
        {
            var viewType = view.GetType();

            if (bindings.ContainsKey(viewType))
            {
                var values = binding.value as object[];
                var aa = values.Length;
                for (var a = 0; a < aa; a++)
                {
                    var mediatorType = values[a] as Type;
                    method(view, mediatorType);
                }
            }
        }

        private IView[] GetViews(IView view)
        {
            var mono = view as MonoBehaviour;
            return mono != null
                ? mono.GetComponentsInChildren<IView>(true)
                : default;
        }

        /// Destroy the Mediator on the provided view object based on the mediatorType
        private object DestroyMediator(IView view, Type mediatorType)
        {
            if (!_viewMediatorLinksDictionary.TryGetValue(view, out var mediator)) return null;

            mediator.OnRemove();
            mediator.BaseMediatedView = null;
            _viewMediatorLinksDictionary.Remove(view);
            return mediator;
        }

        private object EnableMediator(IView view, Type mediatorType)
        {
            if (!_viewMediatorLinksDictionary.TryGetValue(view, out var mediator)) return null;

            mediator.OnEnabledInternal();
            mediator.OnEnabled();
            return mediator;
        }

        private object DisableMediator(IView view, Type mediatorType)
        {
            if (!_viewMediatorLinksDictionary.TryGetValue(view, out var mediator)) return null;

            mediator.OnDisabledInternal();
            mediator.OnDisabled();
            return mediator;
        }
    }
}