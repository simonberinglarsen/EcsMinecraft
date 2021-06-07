namespace ConsoleApp1.Components
{
    using System.Collections.Generic;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class WindowComponent
    {
        public Entity Id { get; set; }

        public int WindowType { get; set; }

        public List<Control> PanelControls { get; set; } = new List<Control>();

        public class Control
        {
            public Control(int id, string text, Vector2i position, Color col = Color.Black)
            {
                Id = id;
                Text = text;
                Position = position;
                Color = col;
            }

            public int Id { get; set; }

            public bool Visible { get; set; } = true;

            public Color Color { get; set; }

            public bool Focus { get; set; }

            public bool Clickable { get; set; } = true;

            public Vector2i Position { get; set; }

            public string Text { get; set; }
        }
    }
}