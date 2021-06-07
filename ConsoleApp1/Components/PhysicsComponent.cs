namespace ConsoleApp1.Components
{
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class PhysicsComponent
    {
        public Entity Id { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 PositionPrev { get; set; }

        public float Height { get; set; }

        public float Width { get; set; }

        public Vector3 Velocity { get; set; }

        public Vector3 Gravity { get; set; } = new Vector3(0, -0.15f, 0);

        public AxisAlignedBoundingBox AABB
        {
            get
            {
                var halftWidth = Width / 2f;
                return new AxisAlignedBoundingBox
                {
                    MinX = Position.X - halftWidth,
                    MinY = Position.Y,
                    MinZ = Position.Z - halftWidth,
                    MaxX = Position.X + halftWidth,
                    MaxY = Position.Y + Height,
                    MaxZ = Position.Z + halftWidth,
                };
            }
        }

        public Vector3 Force { get; set; }
    }
}