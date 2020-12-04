using UnityEngine;
using Utils;

namespace Game.Gameplay.Interfaces
{
    public interface IShapeRandomizer
    {
        (ShapeContainer shape, GameObject block) RandomizeBlock();
    }
}