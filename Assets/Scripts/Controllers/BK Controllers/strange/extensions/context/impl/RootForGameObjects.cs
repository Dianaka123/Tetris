using System;
using strange.extensions.context.api;
using UnityEngine;

namespace strange.extensions.context.impl
{
    [Serializable]
    public class RootForGameObjects : IRootForGameObjects
    {
        [SerializeField] private Transform _transform;

        public Transform ContainerTransform => _transform;
    }
}