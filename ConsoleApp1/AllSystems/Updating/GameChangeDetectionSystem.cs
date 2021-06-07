namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Linq;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class GameChangeDetectionSystem
    {
        public void Run(SettingsComponent settings, InputComponent input, PerformanceComponent performance, Game game)
        {
            if (settings.GameTick % 20 == 0)
            {
                new PerformanceService(performance).ClearAll();
            }

            settings.GameTick++;
            switch (EntityDatabase.SettingsComponent.DayNightCycle)
            {
                case DayNightCycle.Fast:
                    EntityDatabase.LandscapeComponent.LevelOfSunlight = HoursToSunlightLevel(TicksToHours(EntityDatabase.SettingsComponent.GameTick, 1));
                    break;
                case DayNightCycle.Normal:
                    EntityDatabase.LandscapeComponent.LevelOfSunlight = HoursToSunlightLevel(TicksToHours(EntityDatabase.SettingsComponent.GameTick, 20));
                    break;
                case DayNightCycle.AlwaysDay:
                    EntityDatabase.LandscapeComponent.LevelOfSunlight = 1f;
                    break;
                case DayNightCycle.AlwaysNight:
                    EntityDatabase.LandscapeComponent.LevelOfSunlight = 0f;
                    break;
            }

            var gameModeChanged = settings.GameModePrev != settings.GameMode;
            if (gameModeChanged)
            {
                bool previouslyGrabbed = game.CursorGrabbed;
                game.CursorVisible = settings.GameMode != GameMode.InGame;
                game.CursorGrabbed = settings.GameMode == GameMode.InGame;
                if (previouslyGrabbed && !game.CursorGrabbed)
                {
                    game.MousePosition = new Vector2(game.Size.X / 2, game.Size.Y / 2);
                }
            }

            settings.GameModePrev = settings.GameMode;
            input.KeyboardStatePrev = input.KeyboardState;
            input.MouseStatePrev = input.MouseState;

            EntityDatabase.CleanUp();
        }

        private float HoursToSunlightLevel(float hours)
        {
            if (hours < 0 || hours > 24)
            {
                throw new Exception($"A day cannot have {hours} hours!");
            }

            if (hours < 12)
            {
                return hours / 12f;
            }

            return (24 - hours) / 12f;
        }

        private float TicksToHours(float ticks, float minutesPrCycle)
        {
            const double ticksPrSecond = 20f;
            var secondsPrCycle = minutesPrCycle * 60f;
            var ticksPrCycle = Math.Round(ticksPrSecond * secondsPrCycle);
            var ticksOfCycle = ticks % ticksPrCycle;
            var percentageOfDay = ticksOfCycle / ticksPrCycle;
            return (float)(percentageOfDay * 24.0);
        }

        private float MillitaryHoursToPercent(float millitaryHours)
        {
            return (millitaryHours % 2400) / 2400;
        }
    }
}