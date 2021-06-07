namespace ConsoleApp1.AllSystems.Rendering.MeshBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class CubeMeshBuilder : IVoxelMeshBuilder
    {
        public VertexBuffer Build(ChunkCacheService chunkCacheService, Vector3i localPos, Block thisBlock, BlockResource res)
        {
            VertexBuffer newVertices = new VertexBuffer();
            byte[] li = new byte[3 * 3 * 3];
            byte[] sli = new byte[3 * 3 * 3];
            var aabb = thisBlock.BoundingBox;
            Vector3 p1 = new Vector3(aabb.MinX, aabb.MinY, aabb.MaxZ) + localPos;
            Vector3 p2 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MaxZ) + localPos;
            Vector3 p3 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MaxZ) + localPos;
            Vector3 p4 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MaxZ) + localPos;
            Vector3 p5 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MinZ) + localPos;
            Vector3 p6 = new Vector3(aabb.MinX, aabb.MinY, aabb.MinZ) + localPos;
            Vector3 p7 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MinZ) + localPos;
            Vector3 p8 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MinZ) + localPos;

            int i = 0;
            int x, y, z;
            localPos.Deconstruct(out x, out y, out z);
            for (int dy = 1; dy >= -1; dy--)
            {
                for (int dz = 1; dz >= -1; dz--)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        var pos = new Vector3i(x + dx, y + dy, z + dz);
                        li[i] = chunkCacheService.GetLocalLightAt(pos);
                        sli[i] = chunkCacheService.GetLocalSkyLightAt(pos);
                        i++;
                    }
                }
            }

            float ss1 = 1f;
            float ss2 = 0.9f;
            float ss3 = 0.7f;
            float ss4 = 0.6f;
            float lig1, lig2, lig3, lig4, lig5, lig6, lig7, lig8;
            float sky1, sky2, sky3, sky4, sky5, sky6, sky7, sky8;

            // top
            bool faceIsHidden, forceFace;
            Block otherVoxel;
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(0, 1, 0)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig4 = VertLight(0, 1, 3, 4);
                sky4 = VertSkyLight(0, 1, 3, 4);
                lig3 = VertLight(1, 2, 4, 5);
                sky3 = VertSkyLight(1, 2, 4, 5);
                lig8 = VertLight(4, 5, 7, 8);
                sky8 = VertSkyLight(4, 5, 7, 8);
                lig7 = VertLight(3, 4, 6, 7);
                sky7 = VertSkyLight(3, 4, 6, 7);
                var idx = res.Up.TextureIndex;
                var col = res.Up.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss1, lig4, lig3, lig8, lig7, sky4, sky3, sky8, sky7, p4, p3, p8, p7, res.Up.TextureCoords));
            }

            // front
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(0, 0, 1)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig1 = VertLight(9, 10, 18, 19);
                sky1 = VertSkyLight(9, 10, 18, 19);
                lig2 = VertLight(10, 11, 19, 20);
                sky2 = VertSkyLight(10, 11, 19, 20);
                lig3 = VertLight(1, 2, 10, 11);
                sky3 = VertSkyLight(1, 2, 10, 11);
                lig4 = VertLight(0, 1, 9, 10);
                sky4 = VertSkyLight(0, 1, 9, 10);
                var idx = res.South.TextureIndex;
                var col = res.South.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss2, lig1, lig2, lig3, lig4, sky1, sky2, sky3, sky4, p1, p2, p3, p4, res.South.TextureCoords));
            }

            // right
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(1, 0, 0)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig2 = VertLight(11, 14, 20, 23);
                sky2 = VertSkyLight(11, 14, 20, 23);
                lig5 = VertLight(14, 17, 23, 26);
                sky5 = VertSkyLight(14, 17, 23, 26);
                lig8 = VertLight(5, 8, 14, 17);
                sky8 = VertSkyLight(5, 8, 14, 17);
                lig3 = VertLight(2, 5, 11, 14);
                sky3 = VertSkyLight(2, 5, 11, 14);
                var idx = res.East.TextureIndex;
                var col = res.East.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss3, lig2, lig5, lig8, lig3, sky2, sky5, sky8, sky3, p2, p5, p8, p3, res.East.TextureCoords));
            }

            // back
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(0, 0, -1)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig5 = VertLight(16, 17, 25, 26);
                sky5 = VertSkyLight(16, 17, 25, 26);
                lig6 = VertLight(15, 16, 24, 25);
                sky6 = VertSkyLight(15, 16, 24, 25);
                lig7 = VertLight(6, 7, 15, 16);
                sky7 = VertSkyLight(6, 7, 15, 16);
                lig8 = VertLight(7, 8, 16, 17);
                sky8 = VertSkyLight(7, 8, 16, 17);
                var idx = res.North.TextureIndex;
                var col = res.North.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss2, lig5, lig6, lig7, lig8, sky5, sky6, sky7, sky8, p5, p6, p7, p8, res.North.TextureCoords));
            }

            // left
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(-1, 0, 0)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig6 = VertLight(12, 15, 21, 24);
                sky6 = VertSkyLight(12, 15, 21, 24);
                lig1 = VertLight(9, 12, 18, 21);
                sky1 = VertSkyLight(9, 12, 18, 21);
                lig4 = VertLight(0, 3, 9, 12);
                sky4 = VertSkyLight(0, 3, 9, 12);
                lig7 = VertLight(3, 6, 12, 15);
                sky7 = VertSkyLight(3, 6, 12, 15);
                var idx = res.West.TextureIndex;
                var col = res.West.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss3, lig6, lig1, lig4, lig7, sky6, sky1, sky4, sky7, p6, p1, p4, p7, res.West.TextureCoords));
            }

            // bottom
            otherVoxel = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos + new Vector3i(0, -1, 0)));
            forceFace = thisBlock.IsOpaque && !otherVoxel.IsOpaque;
            faceIsHidden = otherVoxel.FullFaceDown && thisBlock.FaceUp;
            if (!faceIsHidden || forceFace)
            {
                lig6 = VertLight(21, 22, 24, 25);
                sky6 = VertSkyLight(21, 22, 24, 25);
                lig5 = VertLight(22, 23, 25, 26);
                sky5 = VertSkyLight(22, 23, 25, 26);
                lig2 = VertLight(19, 20, 22, 23);
                sky2 = VertSkyLight(19, 20, 22, 23);
                lig1 = VertLight(18, 19, 21, 22);
                sky1 = VertSkyLight(18, 19, 21, 22);
                var idx = res.Down.TextureIndex;
                var col = res.Down.Color;
                newVertices.AddBuffer(CubeVertices.FromPointsWithLighting(idx, col * ss4, lig6, lig5, lig2, lig1, sky6, sky5, sky2, sky1, p6, p5, p2, p1, res.Down.TextureCoords));
            }

            return newVertices;

            float VertLight(int a, int b, int c, int d)
            {
                var light = (li[a] + li[b] + li[c] + li[d]) / (4f * LandscapeComponent.MaxLight);
                return light;
            }

            float VertSkyLight(int a, int b, int c, int d)
            {
                var skyLight = (sli[a] + sli[b] + sli[c] + sli[d]) / (4f * LandscapeComponent.MaxLight);
                return skyLight;
            }
        }
    }
}