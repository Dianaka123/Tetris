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
 * @class strange.extensions.mediation.impl.View
 * 
 * Parent class for all your Views. Extends MonoBehaviour.
 * Bubbles its Awake, Start and OnDestroy events to the
 * ContextView, which allows the Context to know when these
 * critical moments occur in the View lifecycle.
 */

using System;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;
using UnityEngine;

namespace strange.extensions.mediation.impl
{
    public class View : MonoBehaviour, IView
    {
        [SerializeField] [HideInInspector] private ContextView _contextView;

        [SerializeField] [HideInInspector] private bool _hasContextView;

        protected bool IsGameObjectDestroyed { get; private set; }

        /// A MonoBehaviour Awake handler.
        /// The View will attempt to connect to the Context at this moment.
        protected virtual void Awake()
        {
            if (autoRegisterWithContext && !registeredWithContext)
            {
#if STRANGE_ANALYSIS
              StrangeDebugger.Measure(() =>  bubbleToContext(this, BubbleType.Add, false), null, name+"_Awake", "bubbleToContext");
#else
                bubbleToContext(this, BubbleType.Add, false);
#endif
            }
        }

        /// A MonoBehaviour Start handler
        /// If the View is not yet registered with the Context, it will 
        /// attempt to connect again at this moment.
        protected virtual void Start()
        {
            if (autoRegisterWithContext && !registeredWithContext)
            {
#if STRANGE_ANALYSIS
                StrangeDebugger.Measure(() =>   bubbleToContext(this, BubbleType.Add, true), null, name+"_Start", "bubbleToContext");
#else
                bubbleToContext(this, BubbleType.Add, true);
#endif
                ;
            }
        }

        /// A MonoBehaviour OnEnable handler
        /// The View will inform the Context that it was enabled
        protected virtual void OnEnable()
        {
#if STRANGE_ANALYSIS
            StrangeDebugger.Measure(() =>   bubbleToContext(this, BubbleType.Enable, false), null, name+"_OnEnable", "bubbleToContext");
#else
            bubbleToContext(this, BubbleType.Enable, false);
#endif
        }

        /// A MonoBehaviour OnDisable handler
        /// The View will inform the Context that it was disabled
        protected virtual void OnDisable()
        {
            bubbleToContext(this, BubbleType.Disable, false);
        }

        /// A MonoBehaviour OnDestroy handler
        /// The View will inform the Context that it is about to be
        /// destroyed.
        protected virtual void OnDestroy()
        {
            IsGameObjectDestroyed = true;
            bubbleToContext(this, BubbleType.Remove, false);
        }

        public MonoBehaviour monoBehaviour => this;

        /// A flag for allowing the View to register with the Context
        /// In general you can ignore this. But some developers have asked for a way of disabling
        /// View registration with a checkbox from Unity, so here it is.
        /// If you want to expose this capability either
        /// (1) uncomment the commented-out line immediately below, or
        /// (2) subclass View and override the autoRegisterWithContext method using your own custom (public) field.
        //[SerializeField]
        //private bool registerWithContext = true;
        public bool autoRegisterWithContext => true;

        public bool registeredWithContext { get; set; }

        /// Recurses through Transform.parent to find the GameObject to which ContextView is attached
        /// Has a loop limit of 100 levels.
        /// By default, raises an Exception if no Context is found.
        public static ContextView FindTheNearestContextView(MonoBehaviour view, out bool hasContextView)
        {
            hasContextView = false;
            const int loopMax = 100;
            var loopLimiter = 0;
            var trans = view.transform;
            while (trans.parent != null && loopLimiter < loopMax)
            {
                loopLimiter++;
                trans = trans.parent;
                var contextView = trans.GetComponent<ContextView>();
                if (contextView != null)
                {
                    hasContextView = true;
                    return contextView;
                }
            }

            return null;
        }


        private void bubbleToContext(MonoBehaviour view,
            BubbleType type,
            bool finalTry)
        {
            if (!_hasContextView) _contextView = FindTheNearestContextView(view, out _hasContextView);

            if (_hasContextView && _contextView.context != null)
            {
                var context = _contextView.context;

                switch (type)
                {
                    case BubbleType.Add:
                        context.AddView(view);
                        registeredWithContext = true;
                        return;
                    case BubbleType.Remove:
                        context.RemoveView(view);
                        return;
                    case BubbleType.Enable:
                        context.EnableView(view);
                        return;
                    case BubbleType.Disable:
                        context.DisableView(view);
                        return;
                    default:
                        throw new ArgumentOutOfRangeException(type.ToString());
                }
            }

            if (finalTry && type == BubbleType.Add)
            {
                //last ditch. If there's a Context anywhere, we'll use it!
                if (Context.firstContext != null)
                {
                    Context.firstContext.AddView(view);
                    registeredWithContext = true;
                    return;
                }

                var msg =
                    "A view couldn't find a context. Loop limit reached. or " +
                    "A view was added with no context. Views must be added into the hierarchy of their ContextView lest all hell break loose.";
                msg += "\nView: " + view;
                throw new MediationException(
                    msg,
                    MediationExceptionType.NO_CONTEXT);
            }
        }

        /// Determines the type of event the View is bubbling to the Context
        protected enum BubbleType
        {
            Add,
            Remove,
            Enable,
            Disable
        }
    }
}