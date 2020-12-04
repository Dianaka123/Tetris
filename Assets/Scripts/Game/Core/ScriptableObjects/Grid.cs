using Game.Core.Interfaces;
using UnityEngine;

namespace Game.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Grid", menuName = "Tools/Grid", order = 0)]
    public class Grid : UnityEngine.ScriptableObject, IGrid
    {
        [SerializeField] private int horizontalCount = 10;
        [SerializeField] private int verticalCount = 20;
        [SerializeField] private Block sizeOfField;
        [SerializeField] private float interval;
        
        public Vector2Int Dimensions => new Vector2Int(horizontalCount, verticalCount);
        
        public float BlockSize => sizeOfField.Size;
        
        public float Interval => interval;
    }
}    