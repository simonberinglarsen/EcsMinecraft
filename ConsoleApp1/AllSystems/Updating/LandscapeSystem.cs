namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LandscapeSystem
    {
        private ChunkColumnProvider provider = new ChunkColumnProvider();
        private SettingsComponent settings;
        private LandscapeComponent landscape;
        private JobPool jobPool;
        private PerformanceService performance;
        private LandscapeService ls = new LandscapeService();

        public LandscapeSystem(JobPool jobPool, PerformanceComponent performance)
        {
            this.jobPool = jobPool;
            this.performance = new PerformanceService(performance);
        }

        public void Run(LandscapeComponent landscape, SettingsComponent settings, PhysicsComponent physics)
        {
            this.settings = settings;
            this.landscape = landscape;
            ls.Bind(landscape);
            var playerPos = physics.Position;
            var playerVoxelPos = ls.ToVector3i(playerPos);
            var playerColPos = ls.ColumnPosFromVoxelPos(playerVoxelPos);
            for (int r = 0; r <= landscape.LandscapeRadiusXZInChunks; r++)
            {
                if (r == 0)
                {
                    UpdateColumn(new Vector2i(0, 0) + playerColPos);
                    continue;
                }

                for (int x = -r; x <= r; x++)
                {
                    UpdateColumn(new Vector2i(x, -r) + playerColPos);
                    UpdateColumn(new Vector2i(x, r) + playerColPos);
                }

                for (int y = -r + 1; y <= r - 1; y++)
                {
                    UpdateColumn(new Vector2i(-r, y) + playerColPos);
                    UpdateColumn(new Vector2i(r, y) + playerColPos);
                }
            }

            var allCols = landscape.ChunkColumns;
            allCols
                .Where(pair => settings.GameTick - pair.Value.LastVisitedTick > 200)
                .ToList()
                .ForEach(pair => allCols.Remove(pair.Key));
            ls.Unbind();
        }

        private void UpdateColumn(Vector2i colPos)
        {
            if (landscape.RequestedColumns.ContainsKey(colPos))
            {
                var token = landscape.RequestedColumns[colPos];
                if (!jobPool.PendingResult(token))
                {
                    return;
                }

                landscape.RequestedColumns.Remove(colPos);
                var res = jobPool.GetResult<ChunkColumn>(token);
                landscape.ChunkColumns[colPos] = res;
            }

            if (!landscape.ChunkColumns.ContainsKey(colPos))
            {
                landscape.RequestedColumns[colPos] = jobPool.AddUpdateJob((args) =>
                {
                    return provider.GetChunkColumn(colPos);
                });
                return;
            }

            // update column logic here...
            var col = landscape.ChunkColumns[colPos];
            col.LastVisitedTick = settings.GameTick;
            UpdateColumn(col, colPos);
        }

        private void UpdateColumn(ChunkColumn col, Vector2i colPos)
        {
        }
    }
}