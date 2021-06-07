namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.AllSystems.Updating;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class RenderSystem : SystemBase, IDisposable
    {
        private bool disposed;
        private float alpha;
        private LandscapeRenderSystem landscapeRenderSystem;
        private ItemRenderSystem itemRenderSystem;
        private LandscapeService ls = new LandscapeService();
        private RenderPipeline pipeline;
        private JobPool jobPool;

        private Dictionary<Entity, InventoryRenderer> inventoryRenderers = new Dictionary<Entity, InventoryRenderer>();
        private PlayerRenderer playerRenderer;
        private AABBRenderer aabbRenderer;
        private PerformanceService performance;
        private Matrix4 view;
        private Matrix4 proj;
        private Matrix4 viewProj;
        private ScreenRenderSystem screenRenderSystem;

        public RenderSystem(JobPool jobPool, PerformanceComponent performance)
        {
            this.pipeline = new RenderPipeline(performance);
            this.jobPool = jobPool;
            landscapeRenderSystem = new LandscapeRenderSystem(jobPool, pipeline, performance);
            itemRenderSystem = new ItemRenderSystem(pipeline);
            screenRenderSystem = new ScreenRenderSystem(pipeline);
            this.performance = new PerformanceService(performance);
        }

        ~RenderSystem()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        public void Run(float alpha)
        {
            performance.Begin(PerformanceService.Section.RENDER_TOTAL);
            this.alpha = alpha;
            CalculateViewAndProjection();
            performance.Begin(PerformanceService.Section.RENDER_WORLD_SPACE);
            RenderDebuggingAABB();
            Join(
                EntityDatabase.ItemComponents,
                EntityDatabase.PhysicsComponents,
                EntityDatabase.ItemRenderComponents,
                (a, b, c) =>
                {
                    itemRenderSystem.Run(alpha, new ItemRenderSystem.Context
                    {
                        Item = a,
                        Physics = b,
                        ItemRenderer = c,
                        ViewProj = viewProj,
                    });
                });
            performance.End(PerformanceService.Section.RENDER_WORLD_SPACE);

            performance.Begin(PerformanceService.Section.RENDER_LANDSCAPE);
            landscapeRenderSystem.Run(
                alpha,
                new LandscapeRenderSystem.Context
                {
                    View = view,
                    Proj = proj,
                    PlayerPhysics = EntityDatabase.PhysicsComponents[EntityDatabase.QueryPlayerEntity()],
                    Settings = EntityDatabase.SettingsComponent,
                    Landscape = EntityDatabase.LandscapeComponent,
                    LandscapeRenderer = EntityDatabase.LandscapeRendererComponent,
                });
            performance.End(PerformanceService.Section.RENDER_LANDSCAPE);

            performance.Begin(PerformanceService.Section.RENDER_ORTHO);
            RenderOrtho();
            performance.End(PerformanceService.Section.RENDER_ORTHO);

            performance.Begin(PerformanceService.Section.RENDER_PIPELINE);
            pipeline.SkyColor = GetSkyColor();
            pipeline.Flush();
            performance.End(PerformanceService.Section.RENDER_PIPELINE);

            performance.Increase(PerformanceService.Section.RENDER_FRAME_COUNT);
            performance.End(PerformanceService.Section.RENDER_TOTAL);
        }

        private void CalculateViewAndProjection()
        {
            Vector2i size = EntityDatabase.SettingsComponent.Size;
            var camera = EntityDatabase.CameraComponent;
            var player = EntityDatabase.PlayerComponents[EntityDatabase.QueryPlayerEntity()];
            const float maxFov = 1.1f;
            var fovPrev = player.IsRunningPrev ? maxFov : 1.0f;
            var fovNow = player.IsRunning ? maxFov : 1.0f;
            var fov = fovPrev + ((fovNow - fovPrev) * alpha);
            proj = Matrix4.CreatePerspectiveFieldOfView(fov, size.X / (float)size.Y, 0.1f, 1000.0f);
            Vector3 posNow = camera.Position;
            Vector3 dirNow = camera.Direction;
            Vector3 posPrev = camera.PositionPrev;
            Vector3 dirPrev = camera.DirectionPrev;
            Vector3 pos = posPrev + ((posNow - posPrev) * alpha);
            Vector3 dir = dirPrev + ((dirNow - dirPrev) * alpha);
            view = Matrix4.LookAt(pos, pos + dir, new Vector3(0, 1, 0));
            viewProj = view * proj;
        }

        private void RenderOrtho()
        {
            Vector2i size = EntityDatabase.SettingsComponent.Size;
            var projOrtho = Matrix4.CreateOrthographicOffCenter(0, size.X, -size.Y, 0, 0.1f, 1000f);
            var viewOrtho = Matrix4.LookAt(new Vector3(0, 0, 1), Vector3.Zero, new Vector3(0, 1, 0));
            var viewProj = viewOrtho * projOrtho;
            Join(
                EntityDatabase.ScreenRenderComponents,
                EntityDatabase.ScreenComponents,
                (a, b) =>
            {
                screenRenderSystem.Run(a, b, viewProj);
            });
            Join(
                EntityDatabase.ScreenOverlayRenderComponents,
                EntityDatabase.ScreenOverlayComponents,
                (a, b) =>
                {
                    screenRenderSystem.Run(a, b, viewProj);
                });

            foreach (var pair in EntityDatabase.InventoryComponents)
            {
                var e = pair.Key;
                var comp = pair.Value;
                if (!inventoryRenderers.ContainsKey(e))
                {
                    inventoryRenderers[e] = new InventoryRenderer(pipeline, comp);
                }

                var renderer = inventoryRenderers[e];
                renderer.Render(viewOrtho, projOrtho);
            }
        }

        private void RenderDebuggingAABB()
        {
            RenderPlayer(view, proj);
            foreach (var physics in EntityDatabase.PhysicsComponents.Values)
            {
                /* RenderAABB(view, proj, physics); */
            }
        }

        private void RenderAABB(Matrix4 view, Matrix4 proj, PhysicsComponent physics)
        {
            if (aabbRenderer == null)
            {
                aabbRenderer = new AABBRenderer(pipeline);
            }

            aabbRenderer.ClearBoxes();
            aabbRenderer.AddBox(new Vector3(1, 1, 0), physics.AABB);
            ls.Bind(EntityDatabase.LandscapeComponent);
            var landscapeAABB = ls.GetSurroundingBoundingBoxes(physics.AABB.Min, physics.AABB.Max);
            foreach (var aabb in landscapeAABB)
            {
                if (physics.AABB.Intersect(aabb))
                {
                    aabbRenderer.AddBox(new Vector3(1, 0, 0), aabb);
                }
                else
                {
                    aabbRenderer.AddBox(new Vector3(0, 1, 0), aabb);
                }
            }

            aabbRenderer.Build();
            aabbRenderer.Render(alpha, view, proj);
        }

        private void RenderPlayer(Matrix4 view, Matrix4 proj)
        {
            if (playerRenderer == null)
            {
                var entity = EntityDatabase.QueryPlayerEntity();
                var player = EntityDatabase.PlayerComponents[entity];
                var physics = EntityDatabase.PhysicsComponents[entity];
                playerRenderer = new PlayerRenderer(physics, player, pipeline);
            }

            playerRenderer.Render(alpha, view, proj);
        }

        private Color4 GetSkyColor()
        {
            float t = EntityDatabase.LandscapeComponent.LevelOfSunlight;
            Vector3 day = new Vector3(0.2f, 0.5f, 1f);
            Vector3 night = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 now = (t * day) + ((1 - t) * night);
            return new Color4(now.X, now.Y, now.Z, 1f);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                pipeline.Dispose();
            }

            disposed = true;
        }
    }
}
