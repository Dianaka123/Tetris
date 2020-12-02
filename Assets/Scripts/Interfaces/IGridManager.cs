using UnityEngine;
using Grid = ScriptableObjects.Grid;

namespace UnityAcademy.TreeOfControllersExample
{
    public interface IGridManager
    {
        Grid Grid { get; }
        void SetBlock(Vector2Int coordinate, GameObject block);
        GameObject GetBlock(Vector2Int coordinate);
    }

    public static class GridManagerExtension
    {
        public static bool CheckVerticalCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate, 
            GameObject[,] blocks, 
            int step)
        {
            var sizeY = blocks.GetUpperBound(1) + 1;
            return 
                gridManager.Grid.CheckGround(coordinate.y, sizeY, step) || 
                CheckCollision(gridManager, coordinate, blocks, new Vector2Int(0, step));
        }

        public static bool CheckHorizontalCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate,
            GameObject[,] blocks,
            int step)
        {
            var sizeX = blocks.GetUpperBound(0) + 1;
            return 
                gridManager.Grid.CheckSide(coordinate.x, sizeX, step) || 
                CheckCollision(gridManager, coordinate, blocks, new Vector2Int(step, 0));
        }
        
        private static bool CheckCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate,
            GameObject[,] blocks,
            Vector2Int step)
        {
            var sizeX = blocks.GetUpperBound(0) + 1;
            var sizeY = blocks.GetUpperBound(1) + 1;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (blocks[x, y] == null) continue;
                    
                    if (gridManager.GetBlock( new Vector2Int(
                        coordinate.x + x + step.x,
                        coordinate.y + y + step.y)) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
    
    
}