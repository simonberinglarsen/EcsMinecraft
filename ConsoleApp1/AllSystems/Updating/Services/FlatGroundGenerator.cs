namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class FlatGroundGenerator
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

        public FlatGroundGenerator(BasicColumn col)
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
                    voxel = BlockId.Grass;
                }
                else if (y < lvl)
                {
                    voxel = BlockId.Stone;
                }

                SetGlobalVoxel(x, y, z, voxel);
            }
        }

        private ushort GetHeightAt(int x, int z)
        {
            return 64;
        }
    }
}