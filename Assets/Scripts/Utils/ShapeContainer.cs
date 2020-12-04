namespace Utils
{
    public struct ShapeContainer
    {
        public readonly string Id;
        public readonly int BlockCount;
        public readonly Shape[] Shapes;

        public ShapeContainer(string id, Shape[] shapes, int blockCount)
        {
            Id = id;
            Shapes = shapes;
            BlockCount = blockCount;
        }
    }
}