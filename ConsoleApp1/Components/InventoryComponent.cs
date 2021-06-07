namespace ConsoleApp1.Components
{
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Entities;

    public class InventoryComponent
    {
        public Entity Id { get; set; }

        public int SelectedSlot { get; set; }

        public ItemStack[] Slots { get; set; } = new ItemStack[8];

        public bool Dirty { get; set; } = true;

        public int ScrollCooldown { get; set; }

        public ItemStack SelectedItemStack
        {
            get
            {
                return Slots[SelectedSlot];
            }
        }

        public bool HasOpenSlots => Slots.Any(s => s.BlockId == BlockId.Void);
    }
}