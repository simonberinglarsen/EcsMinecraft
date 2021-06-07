namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class PlayerRenderer
    {
        private bool disposed;
        private PlayerComponent player;
        private PhysicsComponent physics;
        private RenderPipeline pipeline;
        private VAOWrapper vao = new VAOWrapper();
        private CubeRenderer cubeRenderer;

        public PlayerRenderer(PhysicsComponent physics, PlayerComponent player, RenderPipeline pipeline)
        {
            this.player = player;
            this.physics = physics;
            this.pipeline = pipeline;
            this.cubeRenderer = new CubeRenderer(pipeline);
        }

        ~PlayerRenderer()
        {
            Dispose(false);
        }

        public void Build()
        {
            if (false)
            {
                VertexBuffer newVertices = new VertexBuffer();

                const float w = 0.5f;
                const float h = 1.8f;
                Vector3 p1 = new Vector3(-w, 0, w);
                Vector3 p2 = new Vector3(w, 0, w);
                Vector3 p3 = new Vector3(w, h, w);
                Vector3 p4 = new Vector3(-w, h, w);
                Vector3 p5 = new Vector3(w, 0, -w);
                Vector3 p6 = new Vector3(-w, 0, -w);
                Vector3 p7 = new Vector3(-w, h, -w);
                Vector3 p8 = new Vector3(w, h, -w);

                Vector3 col1 = new Vector3(1, 0, 1);
                Vector3 col2 = new Vector3(1, 0, 1) * 0.8f;
                Vector3 col3 = new Vector3(1, 0, 1) * 0.6f;
                AddFace(p1, p2, p3, p4, col2);
                AddFace(p2, p5, p8, p3, col3);
                AddFace(p5, p6, p7, p8, col2);
                AddFace(p6, p1, p4, p7, col3);
                AddFace(p4, p3, p8, p7, col1);
                AddFace(p6, p5, p2, p1, col1);
                var shader = pipeline.SimpleShader;
                vao
                    .CreateAndBind(newVertices)
                    .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                    .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.Float)
                    .ApplyAndUnbind();

                void AddFace(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 col)
                {
                    newVertices.AddFloatRange(new float[]
                    {
                    p1.X, p1.Y, p1.Z, col.X, col.Y, col.Z,
                    p2.X, p2.Y, p2.Z, col.X, col.Y, col.Z,
                    p3.X, p3.Y, p3.Z, col.X, col.Y, col.Z,
                    p1.X, p1.Y, p1.Z, col.X, col.Y, col.Z,
                    p3.X, p3.Y, p3.Z, col.X, col.Y, col.Z,
                    p4.X, p4.Y, p4.Z, col.X, col.Y, col.Z,
                    });
                }
            }

            if (player.SelectedVoxel.HasValue)
            {
                var ws = new LandscapeService();
                ws.Bind(EntityDatabase.LandscapeComponent);
                ushort voxel = ws.GetGlobalVoxelAt(player.SelectedVoxel.Value);
                var block = BlockRepository.Get(voxel);
                if (block.Visible)
                {
                    var aabb = block.BoundingBox.MoveTo(player.SelectedVoxel.Value);
                    cubeRenderer.Build(new Vector3(1, 1, 1), aabb);
                }
            }
        }

        public void Render(float alpha, Matrix4 view, Matrix4 proj)
        {
            if (player.SelectedVoxelPrev != player.SelectedVoxel)
            {
                Build();
            }

            if (vao.Created)
            {
                Vector3 pos = physics.PositionPrev + ((physics.Position - physics.PositionPrev) * alpha);
                Matrix4 translate = Matrix4.CreateTranslation(pos);
                Matrix4 model = translate;
                Matrix4 mvp = model * view * proj;
                pipeline.Enqueue(DrawPipeline.SimpleWithDepthTest, new PipelineCommand(mvp, vao));
            }

            cubeRenderer.Render(alpha, view, proj);
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