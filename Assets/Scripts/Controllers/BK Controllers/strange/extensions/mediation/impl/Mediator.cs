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
 * @class strange.extensions.mediation.impl.Mediator
 * 
 * Base class for all Mediators.
 * 
 * @see strange.extensions.mediation.api.IMediationBinder
 */

using System.Collections;
using strange.extensions.mediation.api;
using UnityEngine;

namespace strange.extensions.mediation.impl
{
    public abstract class Mediator
    {
        private Coroutine _updateCoroutine;
        private int? _updateCoroutineId;
        protected internal IView BaseMediatedView;

        /**
         * Fires after all injections satisifed.
         *
         * Override and place your initialization code here.
         */
        public virtual void OnRegister()
        {
        }

        /**
         * Fires on removal of view.
         *
         * Override and place your cleanup code here
         */
        public virtual void OnRemove()
        {
        }

        /**
         * Fires on enabling of view.
         */
        public virtual void OnEnabled()
        {
        }

        /**
         * Fires on disabling of view.
         */
        public virtual void OnDisabled()
        {
        }

        internal void OnEnabledInternal()
        {
            if (IsValidCoroutine()) return;

            _updateCoroutine = StartCoroutine(UpdateCoroutine());
        }

        internal void OnDisabledInternal()
        {
            if (_updateCoroutine != null) StopCoroutine(_updateCoroutine);

            _updateCoroutine = null;
        }

        #region Monobehavior extensions

        public GameObject gameObject => BaseMediatedView?.gameObject;

        public Transform transform
        {
            get
            {
                if (gameObject != null) return gameObject.transform;

                return null;
            }
        }

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                yield return null;

                Update();
            }
        }

        protected static void Destroy(GameObject obj)
        {
            if (obj != null) Object.Destroy(obj);
        }

        private bool IsValidCoroutine()
        {
            return _updateCoroutine != null && _updateCoroutineId != null &&
                   BaseMediatedView?.monoBehaviour.GetInstanceID() == _updateCoroutineId;
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            _updateCoroutineId = BaseMediatedView?.monoBehaviour.GetInstanceID();
            return BaseMediatedView?.monoBehaviour.StartCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine coroutine)
        {
            _updateCoroutineId = null;
            BaseMediatedView?.monoBehaviour.StopCoroutine(coroutine);
        }

        protected void StopCoroutine(IEnumerator enumerator)
        {
            BaseMediatedView?.monoBehaviour.StopCoroutine(enumerator);
        }

        protected void StopAllCoroutines()
        {
            BaseMediatedView?.monoBehaviour.StopAllCoroutines();
        }

        protected virtual void Update()
        {
        }

        protected T GetComponent<T>()
        {
            return gameObject == null
                ? default
                : gameObject.GetComponent<T>();
        }

        #endregion
    }
}