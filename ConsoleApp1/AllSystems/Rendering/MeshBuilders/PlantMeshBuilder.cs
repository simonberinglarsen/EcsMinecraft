namespace ConsoleApp1.AllSystems.Rendering.MeshBuilders
{
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using OpenTK.Mathematics;

    public class PlantMeshBuilder : IVoxelMeshBuilder
    {
        public VertexBuffer Build(ChunkCacheService chunkCacheService, Vector3i localPos, Block thisBlock, BlockResource res)
        {
            VertexBuffer newVertices = new VertexBuffer();
            var aabb = thisBlock.BoundingBox;

            Vector3 p1 = new Vector3(aabb.MinX, aabb.MinY, aabb.MaxZ) + localPos;
            Vector3 p2 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MaxZ) + localPos;
            Vector3 p3 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MaxZ) + localPos;
            Vector3 p4 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MaxZ) + localPos;
            Vector3 p5 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MinZ) + localPos;
            Vector3 p6 = new Vector3(aabb.MinX, aabb.MinY, aabb.MinZ) + localPos;
            Vector3 p7 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MinZ) + localPos;
            Vector3 p8 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MinZ) + localPos;

            var li = chunkCacheService.GetLocalLightAt(localPos) / (float)LandscapeComponent.MaxLight;
            var sli = chunkCacheService.GetLocalSkyLightAt(localPos) / (float)LandscapeComponent.MaxLight;
            var idx = res.East.TextureIndex;
            var col = res.East.Color;
            newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col, li, li, li, li, sli, sli, sli, sli, p6, p2, p3, p7, res.East.TextureCoords));
            idx = res.West.TextureIndex;
            col = res.West.Color;
            newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col, li, li, li, li, sli, sli, sli, sli, p1, p5, p8, p4, res.West.TextureCoords));

            return newVertices;
        }
    }
}