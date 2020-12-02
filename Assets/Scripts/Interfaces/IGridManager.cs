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
        public static bool CheckVerticalCollision(this IGridManager gridManager, Vector2Int coordinate, GameObject[,] blocks, int step)
        {
            var sizeX = blocks.GetUpperBound(0) + 1;
            var sizeY = blocks.GetUpperBound(1) + 1;
            
            if (gridManager.Grid.CheckGround(coordinate.y, sizeY, step))
            {
                return true;
            }
            
            for (int x = 0; x < sizeX ; x++)
            {
                for (int y = sizeY - 1; y >= 0; y--)
                {
                    if (blocks[x, y] == null) continue;
                    
                    if (gridManager.GetBlock( new Vector2Int(
                        coordinate.x + x,
                        coordinate.y + y + step)) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}