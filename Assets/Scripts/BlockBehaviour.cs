using System;
using System.Collections;
using UnityAcademy.TreeOfControllersExample;
using UnityEngine;
using Grid = ScriptableObjects.Grid;

namespace DefaultNamespace
{
    public class BlockBehaviour : MonoBehaviour
    {
        public IGridManager GridManager { get; set; }
        public Shape Shape { get; set; }
        public GameObject Block { get; set; }
        public ISpawnManager SpawnManager { get; set; }

        private Grid _grid => GridManager.Grid;

        private Coroutine horizontalMovingCoroutine;
        private Vector2Int gridCoordinate;
        private GameObject[,] blocks;
        
        private void Start()
        {           
            blocks = new GameObject[Shape.Size.x, Shape.Size.y];
            var spawnGridPointX = (_grid.HorizontalCount - Shape.Size.x) / 2;
            gridCoordinate = new Vector2Int(spawnGridPointX, 0);
            
            InstantiateShape();
            
            UpdatePosition();
            StartCoroutine(MoveVertical());
        }
        
        private void FixedUpdate()
        {
            var horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                horizontalMovingCoroutine = horizontalMovingCoroutine ?? StartCoroutine(MoveHorizontal());
            }
        }

        private IEnumerator MoveHorizontal()
        {
            var horizontal = Input.GetAxis("Horizontal");
            while (horizontal != 0)
            {
                var step = Math.Sign(horizontal);
                if (!GridManager.CheckHorizontalCollision(gridCoordinate, blocks, step))
                {
                    gridCoordinate.x += step;
                    UpdatePosition();
                }

                yield return new WaitForSeconds(0.5f);
                horizontal = Input.GetAxis("Horizontal");
            }
            horizontalMovingCoroutine = null;
        }

        private IEnumerator MoveVertical()
        {
            while (true)
            {
                float interval = Input.GetAxis("Vertical") < 0 ? 0.25f : 0.5f ;

                yield return new WaitForSeconds(interval);
                if (GridManager.CheckVerticalCollision(gridCoordinate, blocks, 1))
                {
                    RegisterShape();
                    SpawnManager.Spawn();
                    Destroy(gameObject);
                }
                gridCoordinate.y++;
                UpdatePosition();
            }
        }

        private void InstantiateShape()
        {
            for (var y = 0; y < Shape.Size.y; y++)
            {
                for (var x = 0; x < Shape.Size.x; x++)
                {
                    var bit = Shape.GetBlock(x, y);
                    if (bit)
                    {
                        var spawnPoint = _grid.GetBlockCoordinate(new Vector2Int(x, y));
                        var block = Instantiate(
                            Block,
                            spawnPoint,
                            Quaternion.identity,
                            gameObject.transform);
                        blocks[x, y] = block;
                    }
                }
            }
        }

        private void RegisterShape()
        {
            for (int y = 0; y < Shape.Size.y; y++)
            {
                for (int x = 0; x < Shape.Size.x; x++)
                {
                    var currentBlock = blocks[x, y];
                    GridManager.SetBlock(
                        new Vector2Int(gridCoordinate.x + x, gridCoordinate.y + y),
                        currentBlock);
                    if (currentBlock != null)
                        currentBlock.transform.parent = null;
                }
            }
        }

        private void UpdatePosition() => transform.position = _grid.GetWorldCoordinate(gridCoordinate);
    }
}