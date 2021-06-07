namespace ConsoleApp1.Components
{
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class PlayerComponent
    {
        public Entity Id { get; set; }

        public Vector3 Direction { get; set; } = new Vector3(0, 0, -1);

        public float Pitch { get; set; } = .5f;

        public Vector3 EyePosition { get; set; }

        public Vector3i? SelectedVoxel { get; set; }

        public Vector3i? SelectedVoxelPrev { get; set; }

        public bool IsRunning { get; set; }

        public bool IsRunningPrev { get; set; }
    }
}