using Game.Core.Interfaces;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Game.Core.Managers
{
    public class GridManager: View, IGridManager
    {
        [SerializeField] private ScriptableObjects.Grid grid; 

        public IGrid Grid => grid;
        
        private GameObject[,] _gameGrid;
        
        protected override void Awake()
        {
            base.Awake();
            var dimensions = grid.Dimensions;
            _gameGrid = new GameObject[dimensions.x, dimensions.y];
        }

        public void SetBlock(Vector2Int coordinate, GameObject block) => 
            _gameGrid[coordinate.x, coordinate.y] = block;

        public GameObject GetBlock(Vector2Int coordinate) => 
            _gameGrid[coordinate.x, coordinate.y];
    }
}