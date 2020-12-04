using Models.Interfaces;
using UnityEngine;

namespace Utils
{
    public static class GridExtension
    {
        public static Vector2 GetSize(this IGrid grid) =>
            new Vector2(grid.GetSize(grid.Dimensions.x), grid.GetSize(grid.Dimensions.y));


        public static Vector2 GetCenter(this IGrid grid) => grid.GetSize() / 2;
        
        public static Vector2 GetWorldCoordinate(this IGrid grid, Vector2Int coordinate)
        {
            var center = grid.GetCenter();
            return new Vector2(
                grid.GetSize(coordinate.x) - (center.x - (grid.BlockSize + grid.Interval) / 2), 
                -(grid.GetSize(coordinate.y + 1) - center.y));
        }

        public static Vector2 GetBlockCoordinate(this IGrid grid, Vector2Int coordinate) =>
            new Vector2(
                grid.GetSize(coordinate.x), 
                -(grid.GetSize(coordinate.y)));

        public static bool CheckSide(this IGrid grid, int xCoordinate, int sizeX, int step)
        {
            var newCoordinate = xCoordinate + step;
            return newCoordinate < 0 || newCoordinate + sizeX > grid.Dimensions.x;
        }
        
        public static bool CheckGround(this IGrid grid, int yCoordinate, int sizeY, int step)
        {
            var newCoordinate = yCoordinate + step;
            return newCoordinate < 0 || newCoordinate + sizeY > grid.Dimensions.y;
        }
        
        private static float GetSize(this IGrid grid, float count) => 
            (grid.BlockSize + grid.Interval) * count;
    }
}