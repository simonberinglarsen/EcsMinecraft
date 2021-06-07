namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Updating.Services;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class AABBRenderer
    {
        private bool disposed;
        private VAOWrapper vao = new VAOWrapper();
        private List<AxisAlignedBoundingBox> boxes = new List<AxisAlignedBoundingBox>();
        private List<Vector3> colors = new List<Vector3>();
        private DrawPipeline drawPipeline;
        private RenderPipeline pipeline;

        public AABBRenderer(RenderPipeline pipeline, DrawPipeline drawPipeline = DrawPipeline.LinesWithNoDepthTest)
        {
            this.drawPipeline = drawPipeline;
            this.pipeline = pipeline;
        }

        ~AABBRenderer()
        {
            Dispose(false);
        }

        public void AddBox(Vector3 col, AxisAlignedBoundingBox b)
        {
            boxes.Add(b);
            colors.Add(col);
        }

        public void ClearBoxes()
        {
            boxes.Clear();
            colors.Clear();
        }

        public void Build()
        {
            VertexBuffer newVertices = new VertexBuffer();
            for (int i = 0; i < boxes.Count; i++)
            {
                var col = colors[i];
                var aabb = boxes[i];
                if (drawPipeline == DrawPipeline.LinesWithDepthTest)
                {
                    float smallAmount = 1 / 64f;
                    aabb.MinX -= smallAmount;
                    aabb.MinY -= smallAmount;
                    aabb.MinZ -= smallAmount;
                    aabb.MaxX += smallAmount;
                    aabb.MaxY += smallAmount;
                    aabb.MaxZ += smallAmount;
                }

                Vector3 p1 = new Vector3(aabb.MinX, aabb.MinY, aabb.MaxZ);
                Vector3 p2 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MaxZ);
                Vector3 p3 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MaxZ);
                Vector3 p4 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MaxZ);

                Vector3 p5 = new Vector3(aabb.MaxX, aabb.MinY, aabb.MinZ);
                Vector3 p6 = new Vector3(aabb.MinX, aabb.MinY, aabb.MinZ);
                Vector3 p7 = new Vector3(aabb.MinX, aabb.MaxY, aabb.MinZ);
                Vector3 p8 = new Vector3(aabb.MaxX, aabb.MaxY, aabb.MinZ);

                AddLines(col, new Vector3[] { p1, p2, p3, p4, p1 });
                AddLines(col, new Vector3[] { p5, p6, p7, p8, p5 });
                AddLines(col, new Vector3[] { p1, p6 });
                AddLines(col, new Vector3[] { p2, p5 });
                AddLines(col, new Vector3[] { p3, p8 });
                AddLines(col, new Vector3[] { p4, p7 });
            }

            var shader = pipeline.SimpleShader;
            vao
                .CreateAndBind(newVertices)
                .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.Float)
                .ApplyAndUnbind();

            void AddLines(Vector3 col, Vector3[] ps)
            {
                for (int i = 0; i < ps.Length - 1; i++)
                {
                    var p1 = ps[i];
                    var p2 = ps[i + 1];
                    newVertices.AddFloatRange(new float[]
                    {
                        p1.X, p1.Y, p1.Z, col.X, col.Y, col.Z,
                        p2.X, p2.Y, p2.Z, col.X, col.Y, col.Z,
                    });
                }
            }
        }

        public void Render(float alpha, Matrix4 view, Matrix4 proj)
        {
            Vector3 pos = new Vector3(0f, 0f, 0f);
            Matrix4 translate = Matrix4.CreateTranslation(pos);
            Matrix4 model = translate;
            Matrix4 mvp = model * view * proj;
            pipeline.Enqueue(drawPipeline, new PipelineCommand(mvp, vao));
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
