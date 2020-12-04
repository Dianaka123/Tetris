using UnityEngine;

namespace Utils
{
    public struct Shape
    {
        public readonly Vector2Int Size;
        public readonly Vector2Int Offset;

        /// <summary>
        /// The mask is written in reverse order.
        /// <example>
        /// <para>Litter L we see like - 0b00101011</para>
        /// <para>But we should write - 0b00110101</para>
        /// </example> 
        /// </summary>
        public int Mask { get; private set; }

        public Shape(Vector2Int size, int? predefinedMask = null, Vector2Int? offset = null)
        {
            Mask = predefinedMask ?? 0;
            Size = size;
            Offset = offset ?? Vector2Int.zero;
        }

        public Shape SetBlock(Vector2Int coordinate, bool exists)
        {
            Mask |= (1 << GetRawIndex(coordinate));
            return this;
        }

        public bool GetBlock(Vector2Int coordinate) => (Mask & (1 << GetRawIndex(coordinate))) != 0;

        private int GetRawIndex(Vector2Int coordinate) => coordinate.y * Size.x + coordinate.x;
    }

}