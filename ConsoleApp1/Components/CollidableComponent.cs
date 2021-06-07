namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;

    public class CollidableComponent
    {
        public Entity Id { get; set; }

        public bool OnGround { get; set; }

        public bool CollideWithLandscape { get; set; }
    }
}