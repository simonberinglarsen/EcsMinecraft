namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class InventorySystem
    {
        private static readonly Dictionary<Keys, int> KeyToSelectedSlotMap = new Dictionary<Keys, int>()
        {
            { Keys.D1, 0 },
            { Keys.D2, 1 },
            { Keys.D3, 2 },
            { Keys.D4, 3 },
            { Keys.D5, 4 },
            { Keys.D6, 5 },
            { Keys.D7, 6 },
            { Keys.D8, 7 },
        };

        public void Run(InventoryComponent inventory, InputComponent input)
        {
            if (inventory.ScrollCooldown == 0)
            {
                var scrollDelta = Math.Sign((input.MouseState.Scroll - input.MouseStatePrev.Scroll).Y);
                if (scrollDelta != 0)
                {
                    inventory.ScrollCooldown = 1;
                    inventory.SelectedSlot = (inventory.SelectedSlot + scrollDelta + 8) % 8;
                }
            }
            else
            {
                inventory.ScrollCooldown--;
            }

            foreach (var map in KeyToSelectedSlotMap)
            {
                if (KeyboardService.KeyPressed(input.KeyboardState, input.KeyboardStatePrev, map.Key))
                {
                    inventory.SelectedSlot = map.Value;
                }
            }
        }
    }
}