using System;
using ScriptableObject;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Grid", menuName = "Tools/Grid", order = 0)]
    public class Grid : UnityEngine.ScriptableObject
    {
        [SerializeField] private int horizontalCount = 10;
        [SerializeField] private int verticalCount = 20;
        [SerializeField] private Block sizeOfField;
        [SerializeField] private float interval;
        
        public int HorizontalCount => horizontalCount;
        public int VerticalCount => verticalCount;
        public float SizeOfField => sizeOfField.Size;
        public float Interval => interval;
        
        public float HorizontalSize => GetSize(horizontalCount);
        public float VerticalSize => GetSize(verticalCount);
        public Vector2 Center => new Vector2(HorizontalSize / 2, VerticalSize / 2);
        
        public Vector2 GetWorldCoordinate(Vector2Int coordinate)
        {
            return new Vector2(
                GetSize(coordinate.x) - (Center.x - (SizeOfField + interval) / 2), 
                -(GetSize(coordinate.y + 1) - Center.y));
        }

        public Vector2 GetBlockCoordinate(Vector2Int coordinate)
        {
            return new Vector2(
                GetSize(coordinate.x), 
                -(GetSize(coordinate.y)));
        }
        
        private float GetSize(float count) => (SizeOfField + interval) * count;

        public int ClampHorizontalGridCoordinate(int xCoordinate, int sizeX, int step)
        {
            xCoordinate += step;
            var leftX = Math.Max(xCoordinate, 0);
            var rightX = Math.Min(leftX + (sizeX - 1), HorizontalCount - 1);
            return Math.Min(leftX, rightX - (sizeX - 1));
        }

        public bool CheckGround(int yCoordinate, int sizeY, int step)
        {
            return yCoordinate + sizeY + step > verticalCount;
        }
    }
}    