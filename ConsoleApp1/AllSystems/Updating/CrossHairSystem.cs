namespace ConsoleApp1.AllSystems.Updating
{
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class CrossHairSystem
    {
        private ScreenService scr = new ScreenService();

        public void Run(CrosshairComponent crosshair, ScreenComponent screen)
        {
            screen.Visible = EntityDatabase.SettingsComponent.GameMode == GameMode.InGame;
            if (!screen.Visible)
            {
                return;
            }

            scr.Bind(screen);
            scr.Print(0, 0, "+");
            scr.Unbind();
        }
    }
}