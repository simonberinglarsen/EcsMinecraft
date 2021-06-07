namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LandscapeRenderSystem
    {
        private JobPool jobPool;
        private RenderPipeline pipeline;
        private PerformanceService performance;
        private Context context;
        private int rebuildsLeft;
        private Frustum frustum = new Frustum();
        private Dictionary<Vector2i, ChunkRenderer[]> inactiveColumns = new Dictionary<Vector2i, ChunkRenderer[]>();
        private Vector3 playerPos;

        public LandscapeRenderSystem(JobPool jobPool, RenderPipeline pipeline, PerformanceComponent performanceComponent)
        {
            this.jobPool = jobPool;
            this.pipeline = pipeline;
            performance = new PerformanceService(performanceComponent);
        }

        public void Run(float alpha, Context context)
        {
            this.context = context;
            playerPos = context.PlayerPhysics.Position;
            frustum.CalculateFrustum(context.Proj, context.View);

            performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_INIT_INACTIVE);
            inactiveColumns.Clear();
            foreach (var pair in context.LandscapeRenderer.ChunkRenderers)
            {
                inactiveColumns[pair.Key] = pair.Value;
            }

            performance.End(PerformanceService.Section.RENDER_LANDSCAPE_INIT_INACTIVE);
            performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS);
            RenderColumns();
            performance.End(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS);

            performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_DISPOSE_INACTIVE);
            foreach (var pair in inactiveColumns)
            {
                context.LandscapeRenderer.ChunkRenderers.Remove(pair.Key);
                pair.Value.ToList().ForEach(r => r.Dispose());
            }

            inactiveColumns.Clear();
            performance.End(PerformanceService.Section.RENDER_LANDSCAPE_DISPOSE_INACTIVE);

            this.context = null;
        }

        private void RenderColumns()
        {
            rebuildsLeft = 1;
            var ls = new LandscapeService();
            var playerVoxelPos = ls.ToVector3i(playerPos);
            var playerColPos = ls.ColumnPosFromVoxelPos(playerVoxelPos);
            for (int r = 0; r <= context.Landscape.LandscapeRadiusXZInChunks; r++)
            {
                RenderColumnsInRadius(r, playerColPos);
            }
        }

        private void RenderColumnsInRadius(int r, Vector2i playerColPos)
        {
            int maxRadius = context.Landscape.LandscapeRadiusXZInChunks * context.Landscape.LandscapeRadiusXZInChunks;
            if (r == 0)
            {
                RenderColumn(new Vector2i(0, 0) + playerColPos);
                return;
            }

            for (int x = -r; x <= r; x++)
            {
                if ((x * x) + (r * r) >= maxRadius)
                {
                    continue;
                }

                RenderColumn(new Vector2i(x, -r) + playerColPos);
                RenderColumn(new Vector2i(x, r) + playerColPos);
            }

            for (int y = -r + 1; y <= r - 1; y++)
            {
                if ((y * y) + (r * r) >= maxRadius)
                {
                    continue;
                }

                RenderColumn(new Vector2i(-r, y) + playerColPos);
                RenderColumn(new Vector2i(r, y) + playerColPos);
            }
        }

        private void RenderColumn(Vector2i colPos)
        {
            var landscape = context.Landscape;
            if (!landscape.ChunkColumns.ContainsKey(colPos))
            {
                return;
            }

            if (!landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(-1, 0))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(1, 0))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(0, -1))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(0, 1))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(1, 1))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(1, -1))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(-1, 1))
                || !landscape.ChunkColumns.ContainsKey(colPos + new Vector2i(-1, -1)))
            {
                return;
            }

            inactiveColumns.Remove(colPos);
            var col = landscape.ChunkColumns[colPos];
            performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS);
            RenderChunksInColumn(col, colPos);
            performance.End(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS);
        }

        private void RenderChunksInColumn(ChunkColumn col, Vector2i colPos)
        {
            ChunkRenderer[] renderers = GetOrCreateRenderers(col, colPos);
            for (var i = 0; i < col.Chunks.Length; i++)
            {
                performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE);
                var chunk = col.Chunks[i];
                var isVisible = IsChunkVisible(chunk);
                performance.End(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE);
                if (!isVisible)
                {
                    continue;
                }

                performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE);
                var dist = (context.PlayerPhysics.Position - (chunk.Position + new Vector3(8, 8, 8))).Length / Chunk.ChunkSize;
                performance.End(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE);
                if (dist > context.Landscape.LandscapeRadiusXZInChunks)
                {
                    continue;
                }

                performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_RENDER);
                var renderer = renderers[i];
                if (renderer.NeedRebuild && rebuildsLeft > 0)
                {
                    if (renderer.Build())
                    {
                        rebuildsLeft--;
                    }
                }

                renderer.Render(context.View, context.Proj);
                performance.End(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_RENDER);
            }
        }

        private bool IsChunkVisible(Chunk chunk)
        {
            int x, y, z;
            chunk.Position.Deconstruct(out x, out y, out z);
            return frustum.CubeVsFrustum(x + 8, y + 8, z + 8, 16);
        }

        private ChunkRenderer[] GetOrCreateRenderers(ChunkColumn col, Vector2i renderKey)
        {
            var chunkRenderers = context.LandscapeRenderer.ChunkRenderers;
            if (!chunkRenderers.ContainsKey(renderKey))
            {
                var renderers = new ChunkRenderer[16];
                for (int i = 0; i < 16; i++)
                {
                    renderers[i] = new ChunkRenderer(jobPool, context.Landscape, col.Chunks[i], pipeline);
                }

                chunkRenderers[renderKey] = renderers;
                return renderers;
            }

            return chunkRenderers[renderKey];
        }

        public class Context
        {
            public Matrix4 View { get; set; }

            public Matrix4 Proj { get; set; }

            public PhysicsComponent PlayerPhysics { get; set; }

            public SettingsComponent Settings { get; set; }

            public LandscapeComponent Landscape { get; set; }

            public LandscapeRendererComponent LandscapeRenderer { get; set; }
        }
    }
}