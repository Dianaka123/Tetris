using System;
using System.Collections;
using UnityEngine;
using Grid = ScriptableObjects.Grid;

namespace UnityAcademy.TreeOfControllersExample
{
    public struct BitGrid
    {
        private readonly BitArray bitArray;
        public Vector2Int Size { get; private set; }

        public BitGrid(Vector2Int size)
        {
            Size = size;
            bitArray = new BitArray(size.x * size.y);
        }

        public void SetBit(Vector2Int coordinate, bool exist) =>
            bitArray[coordinate.y * Size.x + coordinate.x] = exist;
        
        public bool ReadBit(Vector2Int coordinate) => bitArray[coordinate.y * Size.x + coordinate.x];
    }
    
    
    public class GridManager: MonoBehaviour, IGridManager
    {
        [SerializeField] private Grid grid; 

        private GameObject[,] _gameGrid;
        
        private void Awake()
        {
            _gameGrid = new GameObject[Grid.HorizontalCount, Grid.VerticalCount];
        }

        public void SetBlock(Vector2Int coordinate, GameObject block) => _gameGrid[coordinate.x, coordinate.y] = block;

        public GameObject GetBlock(Vector2Int coordinate) => _gameGrid[coordinate.x, coordinate.y];

        public Grid Grid => grid;
    }
}