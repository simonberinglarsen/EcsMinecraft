namespace ConsoleApp1.Components
{
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LandscapeRendererComponent
    {
        public Entity Id { get; set; }

        public Dictionary<Vector2i, ChunkRenderer[]> ChunkRenderers { get; } = new Dictionary<Vector2i, ChunkRenderer[]>();
    }
}