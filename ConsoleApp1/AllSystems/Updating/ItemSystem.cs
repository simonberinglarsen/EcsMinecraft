namespace ConsoleApp1.AllSystems.Updating
{
    using ConsoleApp1.Components;

    public class ItemSystem
    {
        public void Run(ItemComponent item)
        {
            item.Age = EntityDatabase.SettingsComponent.GameTick - item.Born;
        }
    }
}