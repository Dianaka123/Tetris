using System.Collections.Generic;
using System.Linq;
using Game.Core.Interfaces;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Block
{
    public class BlockInitializer : MonoBehaviour
    {
        public IGridManager GridManager { get; set; }
        
        public ShapeContainer ShapeContainer { get; set; }
        
        public GameObject Block { get; set; }
        
        public Shape CurrentShape { get; private set; }
        
        public GameObject[,] BlockGrid { get; private set; }
        
        private IGrid _grid => GridManager.Grid;
        private GameObject[] _initialBlocks;
        private int _rotationIndex = -1;
        
        protected virtual void Start()
        {
            _initialBlocks = InstantiateShape().ToArray();
            Rotate();
        }
        
        private IEnumerable<GameObject> InstantiateShape()
        {
            for (var i = 0; i < ShapeContainer.BlockCount; i++)
            {
                yield return Instantiate(
                    Block,
                    gameObject.transform);
            }
        }

        public void Rotate(Vector2Int? coordinate = null)
        {
            var shapes = ShapeContainer.Shapes;
            IncrementRotationIndex();

            if (coordinate != null )
            {
                while (CheckCanNotToRotate(coordinate.Value, _rotationIndex))
                {
                     IncrementRotationIndex();
                }
            }

            void IncrementRotationIndex()
            {
                _rotationIndex++;
                if (_rotationIndex >= shapes.Length)
                {
                    _rotationIndex = 0;
                }
            }
            
            CurrentShape = shapes[_rotationIndex];

            var shapeSize = CurrentShape.Size;
            BlockGrid = new GameObject[shapeSize.x, shapeSize.y];
            var blockIndex = 0;
            for (var x = 0; x < shapeSize.x; x++)
            {
                for (var y = 0; y < shapeSize.y; y++)
                {
                    var bit = CurrentShape.GetBlock(new Vector2Int(x, y));
                    if (!bit)
                    {
                        continue;
                    }

                    var blockCoordinate = _grid.GetBlockCoordinate(new Vector2Int(x, y) + CurrentShape.Offset);
                    var position = transform.position;
                    var block = _initialBlocks[blockIndex];
                    
                    BlockGrid[x, y] = block;
                    block.transform.position = new Vector3(
                        blockCoordinate.x + position.x,
                        blockCoordinate.y + position.y,
                        position.z);
                    blockIndex++;
                }
            }
        }
        
        public void UpdatePosition(Vector2Int gridCoordinate) => 
            transform.position = _grid.GetWorldCoordinate(gridCoordinate);

        private bool CheckCanNotToRotate(Vector2Int coordinate, int shapeIndex)
        {
            return GridManager.CheckVerticalCollision(coordinate, ShapeContainer.Shapes[shapeIndex], 0) ||
                   GridManager.CheckHorizontalCollision(coordinate, ShapeContainer.Shapes[shapeIndex], 0);
        }
    }
}