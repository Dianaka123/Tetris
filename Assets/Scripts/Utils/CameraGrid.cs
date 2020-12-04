using System;
using ScriptableObject;
using UnityEngine;
using Utils;
using Grid = ScriptableObjects.Grid;

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
