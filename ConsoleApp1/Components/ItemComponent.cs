namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;

    public class ItemComponent
    {
        public Entity Id { get; set; }

        public ushort BlockId { get; set; }

        public long Age { get; set; }

        public long Random { get; set; }

        public long Born { get; set; }

        public int Count { get; set; } = 1;
    }
}