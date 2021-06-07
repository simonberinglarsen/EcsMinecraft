namespace ConsoleApp1.AllSystems.Updating.Services
{
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ChunkService
    {
        private Chunk entity;
        private LandscapeService ws = new LandscapeService();

        public void Bind(LandscapeComponent landscape, Chunk entity)
        {
            this.entity = entity;
            ws.Bind(landscape);
        }

        public void Unbind()
        {
            Bind(null, null);
        }

        public ushort GetLocalVoxelAt(Vector3i pos)
        {
            if (
                pos.X < 0 || pos.X >= Chunk.ChunkSize ||
                pos.Y < 0 || pos.Y >= Chunk.ChunkSize ||
                pos.Z < 0 || pos.Z >= Chunk.ChunkSize)
            {
                return ws.GetGlobalVoxelAt(pos + entity.Position);
            }

            return entity.Voxels[pos.X + (pos.Y * Chunk.ChunkSize) + (pos.Z * Chunk.ChunkSizeSquared)];
        }

        public byte GetLocalLightAt(Vector3i pos)
        {
            if (
               pos.X < 0 || pos.X >= Chunk.ChunkSize ||
               pos.Y < 0 || pos.Y >= Chunk.ChunkSize ||
               pos.Z < 0 || pos.Z >= Chunk.ChunkSize)
            {
                return ws.GetGlobalLightAt(pos + entity.Position);
            }

            return entity.Lights[pos.X + (pos.Y * Chunk.ChunkSize) + (pos.Z * Chunk.ChunkSizeSquared)];
        }

        public byte GetLocalSkyLightAt(Vector3i pos)
        {
            if (
               pos.X < 0 || pos.X >= Chunk.ChunkSize ||
               pos.Y < 0 || pos.Y >= Chunk.ChunkSize ||
               pos.Z < 0 || pos.Z >= Chunk.ChunkSize)
            {
                return ws.GetGlobalSkyLightAt(pos + entity.Position);
            }

            return entity.SkyLights[pos.X + (pos.Y * Chunk.ChunkSize) + (pos.Z * Chunk.ChunkSizeSquared)];
        }
    }
}