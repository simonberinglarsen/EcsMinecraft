namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class CubeRenderer
    {
        private bool disposed;
        private VAOWrapper vao = new VAOWrapper();
        private RenderPipeline pipeline;

        public CubeRenderer(RenderPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        ~CubeRenderer()
        {
            Dispose(false);
        }

        public void Build(Vector3 vector3, AxisAlignedBoundingBox aabb)
        {
            float smallAmount = 1 / 128f;
            aabb.MinX -= smallAmount;
            aabb.MinY -= smallAmount;
            aabb.MinZ -= smallAmount;
            aabb.MaxX += smallAmount;
            aabb.MaxY += smallAmount;
            aabb.MaxZ += smallAmount;

            var newVertices = CubeVertices.FromAabbWithCharOverlay(aabb, new Vector4(0f, 0f, 0f, 0.75f));
            var shader = pipeline.CharShader;
            vao
                .CreateAndBind(newVertices)
                .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                .ConfigAddVertices(shader, "aTexCoord", 2, VertexAttribPointerType.Float)
                .ConfigAddVertices(shader, "aColor", 4, VertexAttribPointerType.Float)
                .ApplyAndUnbind();
        }

        public void Render(float alpha, Matrix4 view, Matrix4 proj)
        {
            Vector3 pos = new Vector3(0f, 0f, 0f);
            Matrix4 translate = Matrix4.CreateTranslation(pos);
            Matrix4 model = translate;
            Matrix4 mvp = model * view * proj;
            pipeline.Enqueue(DrawPipeline.CharBlocksWithDepthTest, new PipelineCommand(mvp, vao));
        }

        public void SetBox(Vector3 vector3, AxisAlignedBoundingBox aabb)
        {
            throw new NotImplementedException();
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
                vao.Delete();
            }

            disposed = true;
        }
    }
}
