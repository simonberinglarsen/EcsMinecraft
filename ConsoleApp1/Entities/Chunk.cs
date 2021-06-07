namespace ConsoleApp1.Entities
{
    using System;
    using OpenTK.Mathematics;

    public class Chunk
    {
        public const int ChunkSize = 16;

        public const float ChunkSizeHalf = ChunkSize / 2f;

        public const int ChunkSizeSquared = ChunkSize * ChunkSize;

        public const int ChunkSizeCubed = ChunkSize * ChunkSize * ChunkSize;

        private bool dirty = false;

        public Entity Id { get; set; }

        public Vector3i Position { get; set; } = new Vector3i(0, 0, 0);

        public bool Dirty
        {
            get
            {
                return dirty;
            }

            set
            {
                dirty = value;
                if (dirty)
                {
                    IsEmpty = false;
                }
            }
        }

        public bool IsEmpty { get; private set; } = true;

        public ushort[] Voxels { get; set; } = new ushort[ChunkSizeCubed];

        public byte[] Lights { get; set; } = new byte[ChunkSizeCubed];

        public byte[] SkyLights { get; set; } = new byte[ChunkSizeCubed];

        public Chunk Clone()
        {
            var c = new Chunk
            {
                Id = Id,
                Dirty = Dirty,
                Position = Position,
                Lights = new byte[Lights.Length],
                SkyLights = new byte[SkyLights.Length],
                Voxels = new ushort[Voxels.Length],
            };
            Array.Copy(Lights, c.Lights, Lights.Length);
            Array.Copy(SkyLights, c.SkyLights, SkyLights.Length);
            Array.Copy(Voxels, c.Voxels, Voxels.Length);
            return c;
       }
    }
}