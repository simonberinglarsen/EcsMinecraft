namespace ConsoleApp1.Components
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LandscapeComponent
    {
        public const int LandscapeHeightInChunks = 16;
        public const int MaxLight = 15;

        public int LandscapeRadiusXZInChunks { get; set; } = 12;

        public Entity Id { get; set; }

        public Dictionary<Vector2i, ChunkColumn> ChunkColumns { get; } = new Dictionary<Vector2i, ChunkColumn>();

        public Dictionary<Vector2i, Guid> RequestedColumns { get; } = new Dictionary<Vector2i, Guid>();

        public float LevelOfSunlight { get; set; }
    }
}