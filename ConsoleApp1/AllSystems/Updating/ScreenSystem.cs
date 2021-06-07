namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ScreenSystem
    {
        public void Run(SettingsComponent settings, ScreenComponent screen)
        {
            SnapToGrid(screen, settings.Size);
        }

        private void SnapToGrid(ScreenComponent screen, Vector2i size)
        {
            int x, y;
            switch (screen.Anchoring)
            {
                case Anchor.TopLeft:
                    screen.Position = new Vector3i(0, 0, 0);
                    return;
                case Anchor.Top:
                    x = (int)((size.X - (screen.CharSize * screen.Width)) / 2f);
                    screen.Position = new Vector3i(x, 0, 0);
                    return;
                case Anchor.TopRight:
                    x = (int)(size.X - (screen.CharSize * screen.Width));
                    screen.Position = new Vector3i(x, 0, 0);
                    return;
                case Anchor.Left:
                    y = (int)((size.Y - (screen.CharSize * screen.Height)) / 2f);
                    screen.Position = new Vector3i(0, -y, 0);
                    return;
                case Anchor.Center:
                    x = (int)((size.X - (screen.CharSize * screen.Width)) / 2f);
                    y = (int)((size.Y - (screen.CharSize * screen.Height)) / 2f);
                    screen.Position = new Vector3i(x, -y, 0);
                    return;
                case Anchor.Right:
                    x = (int)(size.X - (screen.CharSize * screen.Width));
                    y = (int)((size.Y - (screen.CharSize * screen.Height)) / 2f);
                    screen.Position = new Vector3i(x, -y, 0);
                    return;
                case Anchor.BottomLeft:
                    y = (int)(size.Y - (screen.CharSize * screen.Height));
                    screen.Position = new Vector3i(0, -y, 0);
                    return;
                case Anchor.Bottom:
                    x = (int)((size.X - (screen.CharSize * screen.Width)) / 2f);
                    y = (int)(size.Y - (screen.CharSize * screen.Height));
                    screen.Position = new Vector3i(x, -y, 0);
                    return;
                case Anchor.BottomRight:
                    x = (int)(size.X - (screen.CharSize * screen.Width));
                    y = (int)(size.Y - (screen.CharSize * screen.Height));
                    screen.Position = new Vector3i(x, -y, 0);
                    return;
            }

            throw new Exception("Unknown Anchor");
        }
    }
}