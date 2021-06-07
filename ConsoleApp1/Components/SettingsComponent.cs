namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class SettingsComponent
    {
        public const int DebugPages = 3;

        public long GameTick { get; set; }

        public GameMode GameMode { get; set; } = GameMode.Paused;

        public GameMode GameModePrev { get; set; } = GameMode.InGame;

        public Vector2i Size { get; set; }

        public DayNightCycle DayNightCycle { get; set; } = DayNightCycle.AlwaysDay;

        public int ShowDebugPage { get; set; } = 0;

        public bool ShowInfo { get; set; }

        public bool ShowHud { get; set; } = true;
    }
}