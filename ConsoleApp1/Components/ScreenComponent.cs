namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ScreenComponent
    {
        public ScreenComponent(int width, int height)
        {
            Width = width;
            Height = height;
            Buffer = new char[Width * Height];
            ColBuffer = new Color[Width * Height];
        }

        public Entity Id { get; set; }

        public float CharSize { get; } = 32f;

        public int Width { get; set; }

        public int Height { get; set; }

        public char[] Buffer { get; set; }

        public Color[] ColBuffer { get; set; }

        public bool Dirty { get; set; } = true;

        public Anchor Anchoring { get; set; } = Anchor.TopLeft;

        public Color BackgroundColor { get; set; } = Color.White;

        public bool Visible { get; set; } = true;

        public Vector3i Position { get; set; }

        public Vector2 ScreenPosition { get; set; } = Vector2.Zero;

        public bool OnlyBlanks { get; set; } = false;

        public bool IsOverlay { get; set; } = false;

        public Rect GetClientRect()
        {
            return new Rect(0 + Position.X, 0 - Position.Y, CharSize * Width, CharSize * Height);
        }
    }
}