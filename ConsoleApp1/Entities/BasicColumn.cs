namespace ConsoleApp1.Entities
{
    using System;
    using OpenTK.Mathematics;

    public class BasicColumn
    {
        private readonly ushort[] data = new ushort[65536];

        public Vector2i Position { get; set; }

        public ushort GetVoxel(int x, int y, int z)
        {
            int i = x << 12 | z << 8 | y;
            return GetVoxel(i);
        }

        public ushort GetVoxel(int index)
        {
            return data[index];
        }

        public void SetVoxel(int x, int y, int z, ushort v)
        {
            int i = x << 12 | z << 8 | y;
            SetVoxel(i, v);
        }

        public void SetVoxel(int index, ushort v)
        {
            data[index] = v;
        }
    }
}