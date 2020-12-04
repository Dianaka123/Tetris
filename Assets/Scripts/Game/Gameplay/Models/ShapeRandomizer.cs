using Game.Gameplay.Interfaces;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Models
{
    public class ShapeRandomizer: IShapeRandomizer
    {
        [Inject]
        public IShapeLoader ShapeLoader { get; set; }
        
        [Inject]
        public IShapeManager ShapeManager { get; set; }
        
        public (ShapeContainer shape, GameObject block) RandomizeBlock()
        {
            var colorCount = ShapeLoader.Count;
            var shapes = ShapeManager.Shapes;
            
            var nextShapeIndex = Random.Range(0, shapes.Length - 1);
            var nextColorIndex = Random.Range(0, colorCount - 1);
            return (
                shape: ShapeManager.Shapes[nextShapeIndex], 
                block: ShapeLoader.LoadBlock(nextColorIndex));
        }
    }
}