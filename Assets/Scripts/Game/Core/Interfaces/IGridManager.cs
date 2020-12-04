using UnityEngine;

namespace Game.Core.Interfaces
{
    public interface IGridManager
    {
        IGrid Grid { get; }
        
        void SetBlock(Vector2Int coordinate, GameObject block);
        
        GameObject GetBlock(Vector2Int coordinate);
    }
}