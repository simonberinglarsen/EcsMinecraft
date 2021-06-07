namespace ConsoleApp1.AllSystems.Rendering.Gpu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTK.Graphics.OpenGL;

    public class VAOWrapper
    {
        private static Dictionary<VertexAttribPointerType, int> sizeMap = new Dictionary<VertexAttribPointerType, int>
        {
            { VertexAttribPointerType.Float, sizeof(float) },
            { VertexAttribPointerType.UnsignedByte, sizeof(byte) },
        };

        private List<VertexAttrib> vertexAttribs = new List<VertexAttrib>();
        private int vboHandle = 0;
        private int vaoHandle = 0;
        private int bufferSize;
        private int vertexCount;

        public VAOWrapper()
        {
        }

        public bool Created => vboHandle > 0;

        public VAOWrapper ConfigAddVertices(Shader shader, string attribName, int size, VertexAttribPointerType dataType, bool normalize = true)
        {
            vertexAttribs.Add(new VertexAttrib
            {
                DataType = dataType,
                Attrib = shader.GetAttribLocation(attribName),
                Size = size,
                Normalize = normalize,
            });

            return this;
        }

        public void ApplyAndUnbind()
        {
            var stride = vertexAttribs.Sum(v => v.Size * sizeMap[v.DataType]);
            var offset = 0;
            for (int i = 0; i < vertexAttribs.Count; i++)
            {
                GL.EnableVertexAttribArray(vertexAttribs[i].Attrib);
                GL.VertexAttribPointer(
                    vertexAttribs[i].Attrib,
                    vertexAttribs[i].Size,
                    vertexAttribs[i].DataType,
                    vertexAttribs[i].Normalize,
                    stride,
                    offset);
                offset += vertexAttribs[i].Size * sizeMap[vertexAttribs[i].DataType];
            }

            vertexCount = bufferSize / stride;
            if (bufferSize % stride != 0)
            {
                throw new Exception("MISSING DATA IN VBO");
            }

            Unbind();
        }

        public VAOWrapper CreateAndBind(VertexBuffer buffer)
        {
            if (Created)
            {
                Delete();
            }

            // create VAO
            vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);

            // attach VBO
            vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Size, buffer.Data, BufferUsageHint.StaticDraw);

            vertexAttribs.Clear();
            bufferSize = buffer.Size;
            return this;
        }

        public void Delete()
        {
            GL.DeleteVertexArray(vaoHandle);
            GL.DeleteBuffer(vboHandle);
            vaoHandle = 0;
            vboHandle = 0;
        }

        public void Draw()
        {
            if (vaoHandle == 0)
            {
                return;
            }

            if (vertexCount == 0)
            {
                throw new Exception("unexpected empty VAO!");
            }

            GL.BindVertexArray(vaoHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        }

        public void DrawLines()
        {
            if (vaoHandle == 0)
            {
                return;
            }

            GL.BindVertexArray(vaoHandle);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertexCount);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private class VertexAttrib
        {
            public int Attrib { get; set; }

            public int Size { get; set; }

            public VertexAttribPointerType DataType { get; set; }

            public bool Normalize { get; set; }
        }
    }
}