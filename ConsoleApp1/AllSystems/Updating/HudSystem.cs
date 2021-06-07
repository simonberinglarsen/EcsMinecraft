namespace ConsoleApp1.AllSystems.Updating
{
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;

    public class HudSystem
    {
        private ScreenService scr = new ScreenService();

        public HudSystem()
        {
        }

        public void Run(SettingsComponent settings, HealthComponent health, ScreenComponent screen, ScreenComponent overlay, InventoryComponent inventory, InputComponent input)
        {
            if (!settings.ShowHud)
            {
                screen.Visible = false;
                return;
            }

            bool changed = screen.Visible != settings.ShowHud;
            screen.Visible = true;
            if (settings.GameTick % 10 == 0 || changed)
            {
                health.Hearts++;
                if (health.Hearts > 10)
                {
                    health.Hearts = 0;
                }

                screen.Dirty = true;
            }

            if (settings.GameTick % 22 == 0)
            {
                health.Hunger++;
                if (health.Hunger > 10)
                {
                    health.Hunger = 0;
                }

                screen.Dirty = true;
            }

            scr.Bind(screen);
            string heartsColorString = string.Empty.PadLeft(health.Hearts).Replace(" ", "i").PadLeft(10).Replace(" ", "a");
            string hungerColorString = string.Empty.PadLeft(health.Hunger).Replace(" ", "e").PadLeft(10).Replace(" ", "a");
            string bodyHeatString = string.Empty.PadLeft(health.Hunger).Replace(" ", "웃").PadLeft(10);
            string armorColorString = string.Empty.PadLeft(8).Replace(" ", "g").PadLeft(10).Replace(" ", "a");
            string hungerString = string.Empty.PadLeft(health.Hunger).Replace(" ", "●").PadLeft(10).Replace(" ", "○");
            scr.Print(0, 0, $"{bodyHeatString}    ☖☖☖☖☖☖☖☖☖☖");
            scr.Print(0, 1, $"♥♥♥♥♥♥♥♥♥♥    {hungerString}");
            scr.Print(0, 2, "▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
            scr.Print(0, 3, "                        ");
            scr.Print(0, 4, "▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
            scr.PrintCol(0, 0, $"iojkppgnmhaaaaa{armorColorString}");
            scr.PrintCol(0, 1, $"{heartsColorString}aaaa{hungerColorString}");
            scr.PrintCol(0, 2, "AAAAAAAAAAAAAAAAAAAAAAAA");
            scr.PrintCol(0, 4, "cccccccccccccccccccccccc");
            int x = inventory.SelectedSlot * 3;
            scr.Print(x, 2, "   ");
            scr.Print(x, 3, "   ");
            scr.Print(x, 4, "   ");
            scr.PrintCol(x, 2, "CCC");
            scr.PrintCol(x, 3, "CCC");
            scr.PrintCol(x, 4, "CCC");
            scr.Bind(overlay);
            for (var i = 0; i < 8; i++)
            {
                var countText = inventory.Slots[i].Count == 0 ? string.Empty : $"{inventory.Slots[i].Count}";
                var c = inventory.SelectedSlot == i ? Entities.Color.Yellow : Entities.Color.LightGrey;
                scr.Print(i * 3, 4, countText.PadLeft(3), c);
            }

            overlay.Dirty = true;
            scr.Unbind();
        }
    }
}