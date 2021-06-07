namespace ConsoleApp1.Entities
{
    using OpenTK.Mathematics;

    public struct Rect
    {
        public Rect(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; set; }

        public bool IsInside(Vector2 position)
        {
            return position.X >= Position.X && position.X <= (Position.X + Size.X)
                && position.Y >= Position.Y && position.Y <= (Position.Y + Size.Y);
        }
    }
}