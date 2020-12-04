using Game.Core;
using Game.Core.Common;
using Game.Core.Interfaces;
using Game.Gameplay.Interfaces;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Block
{
    [RequireComponent(typeof(BlockInitializer))]
    public class BlockMover : MonoBehaviour
    {
        public ISpawnManager SpawnManager { get; set; }

        public ISoundManager SoundManager { get; set; }

        public ShapeVerticalMoveSignal ShapeVerticalMoveSignal { get; set; }

        public ShapeHorizontalMoveSignal ShapeHorizontalMoveSignal { get; set; }

        public ShapeRotateSignal ShapeRotateSignal { get; set; }

        public LineFullSignal LineFullSignal { get; set; }

        public GameOverSignal GameOverSignal { get; set; }

        //TODO:
        public Vector2Int _gridCoordinate;

        private IGrid _grid;
        private IGridManager _gridManager;
        private Coroutine _horizontalMovingCoroutine;
        private BlockInitializer _blockInitializer;

        private void Start()
        {
            _blockInitializer = GetComponent<BlockInitializer>();
            _gridManager = _blockInitializer.GridManager;
            _grid = _gridManager.Grid;
            var currentShape = _blockInitializer.CurrentShape;
            
            var spawnGridPointX = (_grid.Dimensions.x - currentShape.Size.x) / 2;
            _gridCoordinate = new Vector2Int(spawnGridPointX, 0);

            if (_gridManager.CheckIfGameOver(_gridCoordinate, currentShape))
            {
                GameOverSignal.Dispatch();
            }
            UpdatePosition();
            
            ShapeVerticalMoveSignal.AddListener(ShiftVertical);
            ShapeHorizontalMoveSignal.AddListener(ShiftHorizontal);
            ShapeRotateSignal.AddListener(ShiftRotate);
        }

        private void OnDestroy()
        {
            ShapeVerticalMoveSignal.RemoveListener(ShiftVertical);
            ShapeHorizontalMoveSignal.RemoveListener(ShiftHorizontal);
            ShapeRotateSignal.RemoveListener(ShiftRotate);
        }

        private void ShiftRotate(int obj)
        {
            _blockInitializer.Rotate(_gridCoordinate);
            SoundManager.PlaybackSound(SoundType.ShapeRotate);
        }

        private void ShiftVertical(int v)
        {
            SoundManager.PlaybackSound(SoundType.ShapeMove);
            CheckVerticalCollision();
            _gridCoordinate.y += v;
            UpdatePosition();;
        }

        private void ShiftHorizontal(int v)
        {
            if (_gridManager.CheckHorizontalCollision(_gridCoordinate, _blockInitializer.CurrentShape, v)) 
                return;
            SoundManager.PlaybackSound(SoundType.ShapeMove);
            _gridCoordinate.x += v;
            UpdatePosition();
        }

        private void CheckVerticalCollision()
        {
            if (!_gridManager.CheckVerticalCollision(_gridCoordinate, _blockInitializer.CurrentShape, 1)) 
                return;
            
            _gridManager.RegisterShape(_gridCoordinate, _blockInitializer.CurrentShape, _blockInitializer.BlockGrid);
            var destroyedRows = _gridManager.TryCollectFullRows();
            if (destroyedRows.Length != 0)
            {
                LineFullSignal.Dispatch(destroyedRows.Length);
            }
            SoundManager.PlaybackSound(SoundType.Destroy);
            SpawnManager.Spawn();
            Destroy(gameObject);
        }

        private void UpdatePosition() => _blockInitializer.UpdatePosition(_gridCoordinate);
    }
}