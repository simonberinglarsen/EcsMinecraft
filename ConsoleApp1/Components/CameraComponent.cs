namespace ConsoleApp1.Components
{
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class CameraComponent
    {
        public Entity Id { get; set; }

        public Vector3 Direction { get; set; }

        public float Pitch { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 DirectionPrev { get; set; }

        public Vector3 PositionPrev { get; set; }
    }
}