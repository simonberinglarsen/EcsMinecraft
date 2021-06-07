namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class GroundGenerator
    {
        private BasicColumn col;
        private SimplexNoiseOctave groundNoise = new SimplexNoiseOctave(12345);
        private SimplexNoiseOctave groundNoise2 = new SimplexNoiseOctave(23546);
        private SimplexNoiseOctave groundNoise3 = new SimplexNoiseOctave(34567);

        private int range = 1;
        private int colposx;
        private int colposz;
        private int voxelposx;
        private int voxelposz;

        public GroundGenerator(BasicColumn col)
        {
            this.col = col;
            colposx = col.Position.X;
            colposz = col.Position.Y;
            voxelposx = col.Position.X * Chunk.ChunkSize;
            voxelposz = col.Position.Y * Chunk.ChunkSize;
        }

        public void Generate()
        {
            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    PopulateColumn(x + colposx, z + colposz);
                }
            }
        }

        public void PopulateColumn(int originX, int originZ)
        {
            var rnd = RandomFactory.FromColPos(new OpenTK.Mathematics.Vector2i(originX, originZ));
            var xstart = originX * Chunk.ChunkSize;
            var xend = xstart + Chunk.ChunkSize;
            var zstart = originZ * Chunk.ChunkSize;
            var zend = zstart + Chunk.ChunkSize;
            for (int x = xstart; x < xend; x++)
            {
                for (int z = zstart; z < zend; z++)
                {
                    ushort height = GetHeightAt(x, z);
                    FillColumnByHeight(col, x, height, z);
                    SpawnPlant(rnd, x, height, z);
                    SpawnTree(rnd, x, height, z);
                }
            }
        }

        private void SpawnTree(Random rnd, int x, int height, int z)
        {
            if (rnd.NextDouble() >= 0.02)
            {
                return;
            }

            if (height >= 64 && height < 70)
            {
                var treeHeight = rnd.Next(2, 8);
                var crownSize = (treeHeight / 2) + 1;
                var ystart = height + 1;
                HalfCircle(x, ystart + treeHeight - crownSize + 2, z, crownSize, BlockId.Leaves);
                for (int y = 0; y <= treeHeight; y++)
                {
                    SetGlobalVoxel(x, ystart + y, z, BlockId.Log);
                }
            }
        }

        private void SetGlobalVoxel(int gx, int gy, int gz, ushort v)
        {
            int x = gx - voxelposx;
            int z = gz - voxelposz;
            if (x >= 0 && x < Chunk.ChunkSize &&
                z >= 0 && z < Chunk.ChunkSize)
            {
                col.SetVoxel(x, gy, z, v);
            }
        }

        private void SpawnPlant(Random rnd, int gx, int gy, int gz)
        {
            if (rnd.NextDouble() >= 0.15)
            {
                return;
            }

            var type = BlockId.TallGrass;
            if (rnd.NextDouble() <= 0.20)
            {
                type = rnd.NextDouble() >= 0.50 ? BlockId.FlowerDandelion : BlockId.FlowerRose;
            }

            if (gy >= 64 && gy < 70)
            {
                SetGlobalVoxel(gx, gy + 1, gz, type);
            }
        }

        private void FillColumnByHeight(BasicColumn col, int x, int lvl, int z)
        {
            if (x < voxelposx || x >= voxelposx + Chunk.ChunkSize || z < voxelposz || z >= voxelposz + Chunk.ChunkSize)
            {
                return;
            }

            for (int y = 0; y <= lvl; y++)
            {
                ushort voxel = BlockId.Void;
                if (y == lvl)
                {
                    if (y >= 70)
                    {
                        voxel = BlockId.Stone;
                    }
                    else if (y >= 64)
                    {
                        voxel = BlockId.Grass;
                    }
                    else
                    {
                        voxel = BlockId.Sand;
                    }
                }
                else if (y < lvl)
                {
                    if (y >= 70)
                    {
                        voxel = BlockId.Stone;
                    }
                    else if (y > 64)
                    {
                        voxel = BlockId.Dirt;
                    }
                    else
                    {
                        voxel = BlockId.Stone;
                    }
                }
                else if (y < 64)
                {
                    voxel = BlockId.Water;
                }

                SetGlobalVoxel(x, y, z, voxel);
            }
        }

        private ushort GetHeightAt(int x, int z)
        {
            var level = Math.Abs(groundNoise.Noise(x / 160f, z / 160f) * 10f);
            level += Math.Abs(groundNoise2.Noise(x / 80f, z / 80f) * 4f);
            level -= Math.Abs(groundNoise3.Noise(x / 20f, z / 20f) * 2f);

            if (level > 5)
            {
                var a = level - 5;
                level = 5 + Math.Min(25 + a, a * a * a);
            }

            ushort height = (ushort)(64 + level);
            return height;
        }

        private void HalfCircle(int x, int y, int z, int r, ushort v)
        {
            for (int x1 = -r; x1 <= r; x1++)
            {
                for (int y1 = 0; y1 <= r; y1++)
                {
                    for (int z1 = -r; z1 <= r; z1++)
                    {
                        if ((x1 * x1) + (y1 * y1) + (z1 * z1) >= (r * r))
                        {
                            continue;
                        }

                        SetGlobalVoxel(x + x1, y + y1, z + z1, v);
                    }
                }
            }
        }
    }
}