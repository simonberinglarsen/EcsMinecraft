namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;

    public class HealthComponent
    {
        public Entity Id { get; set; }

        public int Hearts { get; set; } = 10;

        public int Hunger { get; set; } = 5;
    }
}