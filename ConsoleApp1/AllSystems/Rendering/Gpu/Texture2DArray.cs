namespace ConsoleApp1.AllSystems.Rendering.Gpu
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using OpenTK.Graphics.OpenGL;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Advanced;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using Image = SixLabors.ImageSharp.Image;

    public class Texture2DArray : IDisposable
    {
        private const int MaxTextures = 200;
        private int handle;
        private bool disposed;
        private Dictionary<string, int> filenameIndexMap = new Dictionary<string, int>();
        private List<byte[]> imagesData = new List<byte[]>();
        private int width;
        private int height;

        public Texture2DArray()
        {
            handle = GL.GenTexture();
            Use();
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        ~Texture2DArray()
        {
            Dispose(false);
        }

        public int Save(string path)
        {
            if (filenameIndexMap.ContainsKey(path))
            {
                return filenameIndexMap[path];
            }

            Image<Rgba32> image = Image.Load<Rgba32>(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            if (imagesData.Count == 0)
            {
                width = image.Width;
                height = image.Height;
            }

            imagesData.Add(ToArray(image));
            int index = imagesData.Count - 1;
            filenameIndexMap[path] = index;
            return index;
        }

        public void Compile()
        {
            Use();
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba, width, height, MaxTextures, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            for (int i = 0; i < imagesData.Count; i++)
            {
                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, width, height, 1, PixelFormat.Rgba, PixelType.UnsignedByte, imagesData[i]);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

            imagesData = null;
            filenameIndexMap = null;
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2DArray, handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteTexture(handle);
                handle = 0;
            }

            disposed = true;
        }

        private byte[] ToArray(Image<Rgba32> image)
        {
            var data = MemoryMarshal.AsBytes(image.GetPixelMemoryGroup()[0].Span).ToArray();
            return data;
        }
    }
}