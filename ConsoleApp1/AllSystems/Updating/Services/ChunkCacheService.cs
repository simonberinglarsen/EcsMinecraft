namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ChunkCacheService
    {
        private const int CacheSize = Chunk.ChunkSize + 2;
        private const int CacheSizeSquared = CacheSize * CacheSize;
        private const int CacheSizeCubed = CacheSize * CacheSize * CacheSize;
        private Chunk main = null;

        private ushort[] Voxels { get; set; } = new ushort[CacheSizeCubed];

        private byte[] Lights { get; set; } = new byte[CacheSizeCubed];

        private byte[] SkyLights { get; set; } = new byte[CacheSizeCubed];

        public void CopyMain()
        {
            ushort voxel;
            for (var x = 0; x < Chunk.ChunkSize + 2; x++)
            {
                for (var y = 0; y < Chunk.ChunkSize + 2; y++)
                {
                    for (var z = 0; z < Chunk.ChunkSize + 2; z++)
                    {
                        bool insideMain = (x > 0 && x < Chunk.ChunkSize + 1)
                            && (y > 0 && y < Chunk.ChunkSize + 1)
                            && (z > 0 && z < Chunk.ChunkSize + 1);
                        if (insideMain)
                        {
                            x--;
                            y--;
                            z--;
                            voxel = main.Voxels[x + (y * Chunk.ChunkSize) + (z * Chunk.ChunkSizeSquared)];
                            var light = main.Lights[x + (y * Chunk.ChunkSize) + (z * Chunk.ChunkSizeSquared)];
                            var skyLight = main.SkyLights[x + (y * Chunk.ChunkSize) + (z * Chunk.ChunkSizeSquared)];
                            x++;
                            y++;
                            z++;
                            Voxels[x + (y * CacheSize) + (z * CacheSizeSquared)] = voxel;
                            Lights[x + (y * CacheSize) + (z * CacheSizeSquared)] = light;
                            SkyLights[x + (y * CacheSize) + (z * CacheSizeSquared)] = skyLight;
                            continue;
                        }

                        voxel = Voxels[x + (y * CacheSize) + (z * CacheSizeSquared)];
                    }
                }
            }

            main = null;
        }

        public void Load(ChunkService chunkService, Chunk chunk)
        {
            this.main = chunk.Clone();
            for (var x = 0; x < Chunk.ChunkSize + 2; x++)
            {
                for (var y = 0; y < Chunk.ChunkSize + 2; y++)
                {
                    for (var z = 0; z < Chunk.ChunkSize + 2; z++)
                    {
                        bool insideMain = (x > 0 && x < Chunk.ChunkSize + 1)
                           && (y > 0 && y < Chunk.ChunkSize + 1)
                           && (z > 0 && z < Chunk.ChunkSize + 1);
                        if (insideMain)
                        {
                            continue;
                        }

                        Vector3i localPos = new Vector3i(x - 1, y - 1, z - 1);
                        var voxel = chunkService.GetLocalVoxelAt(localPos);
                        var light = chunkService.GetLocalLightAt(localPos);
                        var skyLight = chunkService.GetLocalSkyLightAt(localPos);
                        Voxels[x + (y * CacheSize) + (z * CacheSizeSquared)] = voxel;
                        Lights[x + (y * CacheSize) + (z * CacheSizeSquared)] = light;
                        SkyLights[x + (y * CacheSize) + (z * CacheSizeSquared)] = skyLight;
                    }
                }
            }
        }

        public ushort GetLocalVoxelAt(Vector3i pos)
        {
            return Voxels[(pos.X + 1) + ((pos.Y + 1) * CacheSize) + ((pos.Z + 1) * CacheSizeSquared)];
        }

        public byte GetLocalLightAt(Vector3i pos)
        {
            var v = Voxels[(pos.X + 1) + ((pos.Y + 1) * CacheSize) + ((pos.Z + 1) * CacheSizeSquared)];
            var block = BlockRepository.Get(v);
            if (!block.LightPasses)
            {
                return block.EmittedLight;
            }

            var l = Lights[(pos.X + 1) + ((pos.Y + 1) * CacheSize) + ((pos.Z + 1) * CacheSizeSquared)];
            return Math.Max(block.EmittedLight, l);
        }

        public byte GetLocalSkyLightAt(Vector3i pos)
        {
            return SkyLights[(pos.X + 1) + ((pos.Y + 1) * CacheSize) + ((pos.Z + 1) * CacheSizeSquared)];
        }
    }
}