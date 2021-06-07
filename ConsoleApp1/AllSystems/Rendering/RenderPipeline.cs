namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.Components;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public enum DrawPipeline
    {
        BlocksWithDepthTest,
        CharBlocksWithDepthTest,
        SimpleWithDepthTest,
        LinesWithNoDepthTest,
        LinesWithDepthTest,
        TransparentWithDepthTest,
        TransparentNoCull,
        OrthoPetscii,
        OrthoPetsciiOverlay,
        OrthoVoxels,
    }

    public class RenderPipeline : IDisposable
    {
        private Dictionary<DrawPipeline, List<PipelineCommand>> pipelines = new Dictionary<DrawPipeline, List<PipelineCommand>>();
        private Texture texture;
        private bool disposed;
        private PerformanceService performance;

        public RenderPipeline(PerformanceComponent performance)
        {
            this.performance = new PerformanceService(performance);
            foreach (var drawType in Enum.GetValues(typeof(DrawPipeline)).Cast<DrawPipeline>())
            {
                pipelines[drawType] = new List<PipelineCommand>();
            }

            texture = new Texture();
            texture.Load("assets/petscii.png");
            BlockTextureRepository.Load();
            CharShader = new Shader("shaders/char-shader.vert", "shaders/char-shader.frag");
            VoxelShader = new Shader("shaders/voxel-shader.vert", "shaders/voxel-shader.frag");
            SimpleShader = new Shader("shaders/simple-shader.vert", "shaders/simple-shader.frag");
        }

        ~RenderPipeline()
        {
            Dispose(false);
        }

        public Shader VoxelShader { get; }

        public Shader CharShader { get; }

        public Shader SimpleShader { get; }

        public Color4 SkyColor { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        public void Flush()
        {
            Shader shader;

            GL.DepthMask(true);
            GL.ClearColor(SkyColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_BlocksWithDepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            shader = VoxelShader;
            shader.Use();
            shader.SetUniform("levelOfSunlight", EntityDatabase.LandscapeComponent.LevelOfSunlight);
            BlockTextureRepository.BlockTextures.Use();
            Draw(shader, DrawPipeline.BlocksWithDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_BlocksWithDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_SimpleWithDepthTest);
            shader = SimpleShader;
            shader.Use();
            Draw(shader, DrawPipeline.SimpleWithDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_SimpleWithDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_LinesWithDepthTest);
            shader = SimpleShader;
            shader.Use();
            DrawLines(shader, DrawPipeline.LinesWithDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_LinesWithDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_LinesWithNoDepthTest);
            GL.Disable(EnableCap.DepthTest);
            shader = SimpleShader;
            shader.Use();
            DrawLines(shader, DrawPipeline.LinesWithNoDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_LinesWithNoDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_TransparentWithDepthTest);
            GL.Enable(EnableCap.DepthTest);
            shader = VoxelShader;
            shader.Use();
            shader.SetUniform("levelOfSunlight", EntityDatabase.LandscapeComponent.LevelOfSunlight);
            BlockTextureRepository.BlockTextures.Use();
            Draw(shader, DrawPipeline.TransparentWithDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_TransparentWithDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_TransparentNoCull);
            GL.Disable(EnableCap.CullFace);
            shader = VoxelShader;
            shader.Use();
            shader.SetUniform("levelOfSunlight", EntityDatabase.LandscapeComponent.LevelOfSunlight);
            BlockTextureRepository.BlockTextures.Use();
            Draw(shader, DrawPipeline.TransparentNoCull);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_TransparentNoCull);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_CharBlocksWithDepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader = CharShader;
            shader.Use();
            texture.Use();
            Draw(shader, DrawPipeline.CharBlocksWithDepthTest);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_CharBlocksWithDepthTest);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_OrthoPetscii);
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader = CharShader;
            shader.Use();
            texture.Use();
            Draw(shader, DrawPipeline.OrthoPetscii);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_OrthoPetscii);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_OrthoVoxels);
            shader = VoxelShader;
            shader.Use();
            shader.SetUniform("levelOfSunlight", 1f);
            BlockTextureRepository.BlockTextures.Use();
            Draw(shader, DrawPipeline.OrthoVoxels);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_OrthoVoxels);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_OrthoPetsciiOverlay);
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader = CharShader;
            shader.Use();
            texture.Use();
            Draw(shader, DrawPipeline.OrthoPetsciiOverlay);
            performance.End(PerformanceService.Section.RENDER_PIPELINE_OrthoPetsciiOverlay);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_SWAP);
            Game.Instance.Context.SwapBuffers();
            performance.End(PerformanceService.Section.RENDER_PIPELINE_SWAP);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE_CLEAR);
            ClearAllPipelines();
            performance.End(PerformanceService.Section.RENDER_PIPELINE_CLEAR);
        }

        public void Enqueue(DrawPipeline type, PipelineCommand cmd)
        {
            pipelines[type].Add(cmd);
        }

        private void Draw(Shader shader, DrawPipeline type)
        {
            foreach (var cmd in pipelines[type])
            {
                shader.SetUniform("mvp", cmd.Mvp);
                cmd.Vao.Draw();
            }
        }

        private void DrawLines(Shader shader, DrawPipeline type)
        {
            foreach (var cmd in pipelines[type])
            {
                shader.SetUniform("mvp", cmd.Mvp);
                cmd.Vao.DrawLines();
            }
        }

        private void ClearAllPipelines()
        {
            foreach (var drawType in Enum.GetValues(typeof(DrawPipeline)).Cast<DrawPipeline>())
            {
                pipelines[drawType].Clear();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                CharShader.Dispose();
                VoxelShader.Dispose();
                texture.Dispose();
            }

            disposed = true;
        }
    }
}