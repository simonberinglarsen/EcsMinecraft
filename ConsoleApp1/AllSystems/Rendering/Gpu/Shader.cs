namespace ConsoleApp1.AllSystems.Rendering.Gpu
{
    using System;
    using System.IO;
    using System.Text;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class Shader : IDisposable
    {
        private int handle;
        private bool disposed = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource;
            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            string infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (infoLogVert != string.Empty)
            {
                Console.WriteLine(infoLogVert);
            }

            string fragmentShaderSource;
            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
            if (infoLogFrag != string.Empty)
            {
                Console.WriteLine(infoLogFrag);
            }

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }

        ~Shader()
        {
            Dispose(false);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void SetUniform(string name, Matrix4 matrix)
        {
            var location = GL.GetUniformLocation(handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetUniform(string name, float f)
        {
            var location = GL.GetUniformLocation(handle, name);
            GL.Uniform1(location, f);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        public void SetUniform(string name, int textureUnit)
        {
            var location = GL.GetUniformLocation(handle, name);
            GL.Uniform1(location, textureUnit);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteProgram(handle);
                handle = 0;
            }

            disposed = true;
        }
    }
}