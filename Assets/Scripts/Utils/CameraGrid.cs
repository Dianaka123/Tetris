using UnityEngine;
using Grid = Game.Core.ScriptableObjects.Grid;

namespace Utils
{
    [RequireComponent(typeof(Camera))]
    public class CameraGrid : MonoBehaviour
    {
        [SerializeField] private Grid grid;

        private void Awake()
        {
            var camera = GetComponent<Camera>();
            camera.orthographic = true;

            var gridWorldSize = grid.GetSize();
            camera.orthographicSize = gridWorldSize.x;
            camera.aspect = gridWorldSize.x / gridWorldSize.y;
        }
    }
}
