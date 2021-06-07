namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class WindowSystem : SystemBase
    {
        private ScreenService scr = new ScreenService();
        private Dictionary<int, Action> windowTypeSetupMap;
        private Dictionary<int, Action> clickHandlerMap;
        private Action onUpdate;
        private WindowComponent window;
        private SettingsComponent settings;

        public WindowSystem()
        {
            windowTypeSetupMap = new Dictionary<int, Action>()
            {
                { 1, SetupDebugWindow },
            };
        }

        public void SetupDebugWindow()
        {
            window.PanelControls.Clear();
            window.PanelControls.AddRange(new WindowComponent.Control[]
            {
                new WindowComponent.Control(1, " Debug ", new Vector2i(3, 2)),
                new WindowComponent.Control(2, " Info ", new Vector2i(3, 4)),
                new WindowComponent.Control(3, " Hud ", new Vector2i(3, 6)),
                new WindowComponent.Control(4, " X ", new Vector2i(27, 0), Color.Red | Color.Inverted),
                new WindowComponent.Control(5, " X ", new Vector2i(1, 2)) { Clickable = false },
                new WindowComponent.Control(6, " X ", new Vector2i(1, 4)) { Clickable = false },
                new WindowComponent.Control(7, " X ", new Vector2i(1, 6)) { Clickable = false },
                new WindowComponent.Control(10, "Cycle:", new Vector2i(10, 2)) { Clickable = true },
                new WindowComponent.Control(11, "???", new Vector2i(16, 2)) { Clickable = false },
                new WindowComponent.Control(8, "▨▨▨▨▨ f*cking AWESOME! ▨▨▨▨", new Vector2i(0, 0), Color.DarkGrey),
                new WindowComponent.Control(9, "▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔", new Vector2i(0, 1), Color.DarkGrey),
            });
            clickHandlerMap = new Dictionary<int, Action>
            {
                { 1, DebugClick },
                { 2, InfoClick },
                { 3, HudClick },
                { 4, CloseClick },
                { 10, CycleClick },
            };
            onUpdate = DebugSettingsUpdate;
        }

        public void Run(WindowComponent window, ScreenComponent screen, InputComponent input, SettingsComponent settings)
        {
            screen.Visible = settings.GameMode == GameMode.Paused;
            if (!screen.Visible)
            {
                return;
            }

            this.window = window;
            this.settings = settings;
            if (window.WindowType == 0)
            {
                window.WindowType = 1;
                windowTypeSetupMap[window.WindowType]();
            }

            scr.Bind(screen);
            scr.Clear();
            Rect screenRect = screen.GetClientRect();
            var relativeMousePos = input.MouseState.Position - screenRect.Position;
            var leftButtonPressed = input.MouseStatePrev.IsButtonDown(MouseButton.Left) && !input.MouseState.IsButtonDown(MouseButton.Left);
            var controls = window.PanelControls;
            controls.ForEach(c =>
            {
                if (c.Text == null)
                {
                    return;
                }

                var pos = new Vector2(c.Position.X, c.Position.Y) * screen.CharSize;
                var size = new Vector2(c.Text.Length, 1) * screen.CharSize;
                Rect myRect = new Rect(pos.X, pos.Y, size.X, size.Y);
                c.Focus = myRect.IsInside(relativeMousePos);
                if (c.Focus && c.Clickable && leftButtonPressed)
                {
                    if (clickHandlerMap.ContainsKey(c.Id))
                    {
                        clickHandlerMap[c.Id]();
                    }
                }
            });

            controls.ForEach(c =>
            {
                if (!c.Visible)
                {
                    return;
                }

                var col = (c.Focus && c.Clickable) ? Color.Blue | Color.Inverted : c.Color;
                scr.Print(c.Position.X, c.Position.Y, c.Text, col);
            });

            onUpdate();

            scr.Unbind();
            this.window = null;
            this.settings = null;
        }

        private void DebugSettingsUpdate()
        {
            window.PanelControls.Single(control => control.Id == 5).Text = settings.ShowDebugPage.ToString();
            SetIndicatorState(6, settings.ShowInfo);
            SetIndicatorState(7, settings.ShowHud);
            window.PanelControls.Single(control => control.Id == 11).Text = settings.DayNightCycle.ToString();

            void SetIndicatorState(int id, bool enabled)
            {
                var ctrl = window.PanelControls.Single(control => control.Id == id);
                ctrl.Text = enabled ? "●" : "○";
                ctrl.Color = enabled ? Color.DarkGreen : Color.DarkGrey;
            }
        }

        private void CloseClick()
        {
            settings.GameMode = settings.GameMode == GameMode.InGame ? GameMode.Paused : GameMode.InGame;
        }

        private void CycleClick()
        {
            var x = Enum.GetValues(typeof(DayNightCycle)).Cast<DayNightCycle>().ToList();
            int next = x.IndexOf(settings.DayNightCycle) + 1;
            next = next >= x.Count ? 0 : next;
            settings.DayNightCycle = x[next];
        }

        private void DebugClick()
        {
            EntityDatabase.SettingsComponent.ShowDebugPage = (EntityDatabase.SettingsComponent.ShowDebugPage + 1) % (SettingsComponent.DebugPages + 1);
        }

        private void InfoClick()
        {
            EntityDatabase.SettingsComponent.ShowInfo = !EntityDatabase.SettingsComponent.ShowInfo;
        }

        private void HudClick()
        {
            EntityDatabase.SettingsComponent.ShowHud = !EntityDatabase.SettingsComponent.ShowHud;
        }
    }
}