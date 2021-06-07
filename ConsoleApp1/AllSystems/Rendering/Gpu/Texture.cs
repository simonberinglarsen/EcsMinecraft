namespace ConsoleApp1.AllSystems.Rendering.Gpu
{
    using System;
    using System.Collections.Generic;
    using OpenTK.Graphics.OpenGL;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using Image = SixLabors.ImageSharp.Image;

    public class Texture : IDisposable
    {
        private int handle;
        private bool disposed;

        public Texture()
        {
            handle = GL.GenTexture();
            Use();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        ~Texture()
        {
            Dispose(false);
        }

        public void Load(string path)
        {
            // Load the image
            Image<Rgba32> image = Image.Load<Rgba32>(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            var pixels = new List<byte>(4 * image.Width * image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);
                for (int x = 0; x < image.Width; x++)
                {
                    if (row[x].R < 50)
                    {
                        pixels.AddRange(new byte[] { 255, 255, 255, 255 });
                    }
                    else
                    {
                        pixels.AddRange(new byte[] { 255, 255, 255, 0 });
                    }
                }
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
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
    }
}