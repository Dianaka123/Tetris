using System;
using ScriptableObject;
using UnityEngine;
using Grid = ScriptableObjects.Grid;

[RequireComponent(typeof(Camera))]
public class CameraGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private void Awake()
    {
        var camera = GetComponent<Camera>();
        camera.orthographicSize = grid.HorizontalSize;
        camera.aspect = grid.HorizontalSize / grid.VerticalSize;
    }
}
