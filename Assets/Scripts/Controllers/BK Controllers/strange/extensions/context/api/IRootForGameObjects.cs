using UnityEngine;

namespace strange.extensions.context.api
{
    public interface IRootForGameObjects
    {
        Transform ContainerTransform { get; }
    }
}