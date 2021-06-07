namespace ConsoleApp1.Entities
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using OpenTK.Mathematics;

    public class ChunkColumn
    {
        public long LastVisitedTick { get; set; }

        public Chunk[] Chunks { get; set; }
    }
}