namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class ScreenRenderSystem
    {
        private RenderPipeline pipeline;

        public ScreenRenderSystem(RenderPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public void Run(ScreenRenderComponent renderer, ScreenComponent screen, Matrix4 viewProj)
        {
            if (!screen.Visible)
            {
                return;
            }

            if (!renderer.Vao.Created || screen.Dirty)
            {
                ScreenBuilder sb = new ScreenBuilder(screen);
                var vertices = sb.Build();
                var shader = pipeline.CharShader;
                renderer.Vao
                    .CreateAndBind(vertices)
                    .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                    .ConfigAddVertices(shader, "aTexCoord", 2, VertexAttribPointerType.Float)
                    .ConfigAddVertices(shader, "aColor", 4, VertexAttribPointerType.Float)
                    .ApplyAndUnbind();
            }

            Matrix4 scale = Matrix4.CreateScale(screen.CharSize);
            Matrix4 translate = Matrix4.CreateTranslation(screen.Position);
            Matrix4 model = scale * translate;
            Matrix4 mvp = model * viewProj;
            pipeline.Enqueue(
                screen.IsOverlay ? DrawPipeline.OrthoPetsciiOverlay : DrawPipeline.OrthoPetscii,
                new PipelineCommand(mvp, renderer.Vao));
        }
    }
}