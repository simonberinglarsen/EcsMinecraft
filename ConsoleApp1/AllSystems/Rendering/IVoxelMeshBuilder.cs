namespace ConsoleApp1.AllSystems.Rendering
{
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using OpenTK.Mathematics;

    public interface IVoxelMeshBuilder
    {
        VertexBuffer Build(ChunkCacheService chunkCacheService, Vector3i localPos, Block thisBlock, BlockResource res);
    }
}