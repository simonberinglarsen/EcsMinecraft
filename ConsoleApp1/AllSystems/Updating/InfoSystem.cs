namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class InfoSystem
    {
        private ScreenService scr = new ScreenService();

        public void Run(InfoComponent info, ScreenComponent screen)
        {
            var settings = EntityDatabase.SettingsComponent;
            if (!settings.ShowInfo)
            {
                screen.Visible = false;
                return;
            }

            bool changed = screen.Visible != settings.ShowInfo;
            screen.Visible = true;
            if (settings.GameTick % 10 == 0 || changed)
            {
                screen.OnlyBlanks = !screen.OnlyBlanks;
                info.Text = DateTime.Now.ToString("HH:mm:ss").PadRight(10);
            }

            scr.Bind(screen);
            scr.Print(0, 0, info.Text.PadLeft(15), Color.White | Color.Inverted);
            scr.Unbind();
        }
    }
}