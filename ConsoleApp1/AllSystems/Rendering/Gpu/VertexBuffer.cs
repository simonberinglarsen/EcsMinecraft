namespace ConsoleApp1.AllSystems.Rendering.Gpu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTK.Graphics.OpenGL;

    public class VertexBuffer
    {
        private List<byte> data;

        public VertexBuffer(int size = 64)
        {
            data = new List<byte>(size);
        }

        public int Size => data.Count * sizeof(byte);

        public byte[] Data => data.ToArray();

        public void AddFloat3(float f1, float f2, float f3)
        {
            data.AddRange(BitConverter.GetBytes(f1));
            data.AddRange(BitConverter.GetBytes(f2));
            data.AddRange(BitConverter.GetBytes(f3));
        }

        public void AddFloat3AsByte(float f1, float f2, float f3)
        {
            data.Add((byte)(f1 * 255));
            data.Add((byte)(f2 * 255));
            data.Add((byte)(f3 * 255));
        }

        public void AddByte3(byte b1, byte b2, byte b3)
        {
            data.Add(b1);
            data.Add(b2);
            data.Add(b3);
        }

        public void AddFloatRange(float[] floatArray)
        {
            foreach (var f in floatArray)
            {
                data.AddRange(BitConverter.GetBytes(f));
            }
        }

        public void AddFloatRangeAsBytes(float[] floatArray)
        {
            foreach (var f in floatArray)
            {
                data.Add((byte)(f * 255));
            }
        }

        public void AddBuffer(VertexBuffer buffer)
        {
            data.AddRange(buffer.Data);
        }
    }
}