using System.Collections.Generic;
using System.Linq;
using Models.Interfaces;
using UnityEngine;
using Utils;

namespace UnityAcademy.TreeOfControllersExample
{
    public static class GridManagerExtension
    {
        public static int[] TryCollectFullRows(this IGridManager gridManager)
        {
            var grid = gridManager.Grid;
            var dimensions = grid.Dimensions;
            
            var fullRows = gridManager.GetFullRows().ToArray();
            foreach (var row in fullRows)
            {
                for (var x = 0; x < dimensions.x; x++)
                {
                    Object.Destroy(gridManager.GetBlock(new Vector2Int(x, row)));
                    for (var y = row; y >= 1 ; y--)
                    {
                        var currentRowCoordinate = new Vector2Int(x, y);
                        var previousRowCoordinate = new Vector2Int(x, y - 1);
                        
                        var currentBlock = gridManager.GetBlock(previousRowCoordinate);
                        gridManager.SetBlock(currentRowCoordinate, currentBlock);
                        
                        if (currentBlock != null) 
                            currentBlock.transform.position = grid.GetWorldCoordinate(currentRowCoordinate);

                        gridManager.SetBlock(previousRowCoordinate, null);
                    }
                }
            }
            return fullRows;
        }
        
        public static void RegisterShape(
            this IGridManager gridManager,
            Vector2Int coordinate,
            Shape currentShape, 
            GameObject[,] blocks)
        {
            var size = currentShape.Size;
            var sizeX = size.x;
            var sizeY = size.y;
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var currentBlock = blocks[x, y];
                    gridManager.SetBlock(
                        new Vector2Int(
                            coordinate.x + x + currentShape.Offset.x, 
                            coordinate.y + y + currentShape.Offset.y),
                        currentBlock);
                    if (currentBlock != null)
                        currentBlock.transform.parent = null;
                }
            }
        }
        
        public static bool CheckVerticalCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate, 
            Shape shape, 
            int step)
        {
            var sizeY = shape.Size.y;
            return 
                gridManager.Grid.CheckGround(coordinate.y + shape.Offset.y, sizeY, step) || 
                CheckCollision(gridManager, coordinate, shape, new Vector2Int(0, step));
        }

        public static bool CheckHorizontalCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate,
            Shape shape,
            int step)
        {
            var sizeX = shape.Size.x;
            return 
                gridManager.Grid.CheckSide(coordinate.x + shape.Offset.x, sizeX, step) || 
                CheckCollision(gridManager, coordinate, shape, new Vector2Int(step, 0));
        }

        public static Vector2Int GetNearestVerticalCollision(
            this IGridManager gridManager,
            Vector2Int coordinate,
            GameObject[,] blocks)
        {
            var sizeX = blocks.GetUpperBound(0) + 1;
            var sizeY = blocks.GetUpperBound(1) + 1;

            var grid = gridManager.Grid;
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = sizeY - 1; y >= 0; y--)
                {
                    //for (var gridY = )
                }  
            }
            return Vector2Int.down;
        }

        public static IEnumerable<int> GetFullRows(this IGridManager gridManager)
        {
            var grid = gridManager.Grid;
            var dimensions = grid.Dimensions;
            for (var y = 0; y < dimensions.y; y++)
            {
                var isFullRow = true;
                for (var x = 0; x < dimensions.x; x++)
                {
                    if (gridManager.GetBlock(new Vector2Int(x, y)) == null)
                    {
                        isFullRow = false;
                        break;
                    }
                }

                if (isFullRow)
                {
                    yield return y;
                }
            }
        }

        public static bool CheckIfGameOver(this IGridManager gridManager, 
            Vector2Int coordinate, Shape shape)
        {
            if (CheckVerticalCollision(gridManager, coordinate, shape, 1))
            {
                return true;
            }

            return false;
        }
        
        private static bool CheckCollision(
            this IGridManager gridManager, 
            Vector2Int coordinate,
            Shape shape,
            Vector2Int step)
        {
            var sizeX = shape.Size.x;
            var sizeY = shape.Size.y;

            var dimensions = gridManager.Grid.Dimensions;
            var maxX = dimensions.x - 1;
            var maxY = dimensions.y - 1;

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    if (!shape.GetBlock(new Vector2Int(x, y))) continue;

                    var resultX = coordinate.x + x + shape.Offset.x + step.x;
                    var resultY = coordinate.y + y + shape.Offset.y + step.y;

                    resultX = Mathf.Clamp(resultX, 0, maxX);
                    resultY = Mathf.Clamp(resultY, 0, maxY);

                    if (gridManager.GetBlock( new Vector2Int(resultX, resultY)) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}