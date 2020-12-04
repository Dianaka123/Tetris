using UnityEngine;

namespace Game.Gameplay.Interfaces
{
    public interface IShapeLoader 
    {
        int Count { get; }
        
        GameObject LoadBlock(int index);
    }
}