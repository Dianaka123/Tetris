using UnityEngine;

namespace Models.Interfaces
{
    public interface IGrid
    {
        Vector2Int Dimensions { get; }

        float BlockSize { get; }
        
        float Interval { get; }
    }
}