namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class GameSettingsSystem
    {
        public void Run(SettingsComponent settings, InputComponent inputComponent, Game game)
        {
            inputComponent.KeyboardState = game.KeyboardState.GetSnapshot();
            inputComponent.MouseState = game.MouseState.GetSnapshot();
            var keyboardState = inputComponent.KeyboardState;
            var keyboardStatePrev = inputComponent.KeyboardStatePrev;
            if (KeyboardService.KeyPressed(keyboardState, keyboardStatePrev, Keys.Escape))
            {
                settings.GameMode = settings.GameMode == GameMode.InGame ? GameMode.Paused : GameMode.InGame;
            }

            if (KeyboardService.KeyPressed(keyboardState, keyboardStatePrev, Keys.F11))
            {
                if (game.WindowState != OpenTK.Windowing.Common.WindowState.Fullscreen)
                {
                    game.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
                }
                else
                {
                    game.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
                }
            }
        }
    }
}