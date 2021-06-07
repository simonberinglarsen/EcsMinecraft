namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;
    using static ConsoleApp1.Entities.ChunkColumn;

    public class ChunkColumnProvider
    {
        public ChunkColumnProvider()
        {
        }

        public ChunkColumn GetChunkColumn(Vector2i colPos)
        {
            BasicColumn basicCol = new BasicColumn();
            basicCol.Position = colPos;

            Decorate(basicCol);

            var col = ConvertToChunkColumn(basicCol);
            return col;
        }

        private void Decorate(BasicColumn col)
        {
            new GroundGenerator(col).Generate();
            /* new FlatGroundGenerator(col).Generate(); */
            new CaveGenerator(col).Generate();
        }

        private ChunkColumn ConvertToChunkColumn(BasicColumn basicCol)
        {
            var colPos = basicCol.Position;
            var chunks = new Chunk[LandscapeComponent.LandscapeHeightInChunks];
            byte[] skyLight = new byte[Chunk.ChunkSizeSquared];
            Array.Fill(skyLight, (byte)LandscapeComponent.MaxLight);
            for (int i = chunks.Length - 1; i >= 0; i--)
            {
                Chunk c = new Chunk();
                c.Position = new Vector3i(colPos.X * Chunk.ChunkSize, i * Chunk.ChunkSize, colPos.Y * Chunk.ChunkSize);
                chunks[i] = c;
                bool hasVoxels = false;
                for (int y = Chunk.ChunkSize - 1; y >= 0; y--)
                {
                    var yofs = i * Chunk.ChunkSize;
                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        for (int z = 0; z < Chunk.ChunkSize; z++)
                        {
                            var v = basicCol.GetVoxel(x, y + yofs, z);
                            var currentSkylight = skyLight[x + (z * Chunk.ChunkSize)];
                            var block = BlockRepository.Get(v);
                            if (currentSkylight > 0)
                            {
                                if (!block.LightPasses)
                                {
                                    skyLight[x + (z * Chunk.ChunkSize)]--;
                                }
                                else if (currentSkylight < LandscapeComponent.MaxLight)
                                {
                                    skyLight[x + (z * Chunk.ChunkSize)]--;
                                }
                            }

                            byte actualSkyLight = 0;
                            if (block.LightPasses)
                            {
                                actualSkyLight = skyLight[x + (z * Chunk.ChunkSize)];
                            }

                            c.SkyLights[x + (y * Chunk.ChunkSize) + (z * Chunk.ChunkSizeSquared)] = actualSkyLight;
                            c.Voxels[x + (y * Chunk.ChunkSize) + (z * Chunk.ChunkSizeSquared)] = v;
                            hasVoxels = hasVoxels || v != BlockId.Void;
                        }
                    }
                }

                c.Dirty = hasVoxels;
            }

            var col = new ChunkColumn();
            col.Chunks = chunks;
            return col;
        }
    }
}