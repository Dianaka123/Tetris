using Game.Gameplay.Interfaces;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Models
{
    public class ShapeManager: IShapeManager
    {
        private static readonly ShapeContainer[] _shapes = new []
        {
            new ShapeContainer("L", new Shape[]
            {
                new Shape( new Vector2Int(2, 3), 0b00110101),
                new Shape( new Vector2Int(3, 2), 0b00001111),
                new Shape( new Vector2Int(2, 3), 0b00110101, new Vector2Int(0, -2)), // TODO
                new Shape( new Vector2Int(3, 2), 0b00001111, new Vector2Int(-2, 0)) // TODO
            }, 4),
            new ShapeContainer("I", new Shape[]
            {
                new Shape(new Vector2Int(1, 4), 0b00001111),
                new Shape(new Vector2Int(4, 1), 0b00001111),
                new Shape(new Vector2Int(1, 4), 0b00001111, new Vector2Int(0, -3)),
                new Shape(new Vector2Int(4, 1), 0b00001111, new Vector2Int(-3, 0))
            }, 4),
            new ShapeContainer("J", new Shape[]
            {
                new Shape(new Vector2Int(2, 3),  0b00111010),
                new Shape(new Vector2Int(3, 2), 0b00111001),
                new Shape(new Vector2Int(2, 3), 0b00111010, new Vector2Int(0, -2)),
                new Shape(new Vector2Int(3, 2), 0b00111001, new Vector2Int(-2, 0))
            }, 4),
            new ShapeContainer("O", new Shape[]
            {
                new Shape(new Vector2Int(2, 2),  0b00001111)
            }, 4),
            new ShapeContainer("T", new Shape[]
            {
                new Shape(new Vector2Int(3, 2),  0b00010111),
                new Shape(new Vector2Int(2, 3), 0b00011101),
                new Shape(new Vector2Int(3, 2), 0b00010111, new Vector2Int(0, -1)),
                new Shape(new Vector2Int(2, 3), 0b00011101, new Vector2Int(-1, 0))
            }, 4),
            new ShapeContainer("Z", new Shape[]
            {
                new Shape(new Vector2Int(3, 2),  0b00110011),
                new Shape(new Vector2Int(2, 3), 0b00011110),
                new Shape(new Vector2Int(3, 2), 0b00110011, new Vector2Int(0, -1)),
                new Shape(new Vector2Int(2, 3), 0b00011110, new Vector2Int(-1, 0))
            }, 4),
            new ShapeContainer("S", new Shape[]
            {
                new Shape(new Vector2Int(3, 2),  0b00011110),
                new Shape(new Vector2Int(2, 3), 0b00110011),
                new Shape(new Vector2Int(3, 2), 0b00011110, new Vector2Int(0, -1)),
                new Shape(new Vector2Int(2, 3), 0b00110011, new Vector2Int(-1, 0))
            }, 4)
        };
        
        public ShapeContainer[] Shapes => _shapes;
    }
}