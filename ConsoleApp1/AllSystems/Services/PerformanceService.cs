namespace ConsoleApp1.AllSystems.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class PerformanceService
    {
        private PerformanceComponent component;

        public PerformanceService(PerformanceComponent component)
        {
            this.component = component;
        }

        public enum Section
        {
            RENDER_TOTAL,
            RENDER_FRAME_COUNT,
            RENDER_WORLD_SPACE,
            RENDER_LANDSCAPE,
            RENDER_LANDSCAPE_INIT_INACTIVE,
            RENDER_LANDSCAPE_RENDER_COLS,
            RENDER_LANDSCAPE_DISPOSE_INACTIVE,
            RENDER_LANDSCAPE_RENDER_COLS_CHUNKS,
            RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE,
            RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_RENDER,
            RENDER_ORTHO,
            RENDER_PIPELINE,
            RENDER_PIPELINE_BlocksWithDepthTest,
            RENDER_PIPELINE_SimpleWithDepthTest,
            RENDER_PIPELINE_LinesWithDepthTest,
            RENDER_PIPELINE_TransparentWithDepthTest,
            RENDER_PIPELINE_TransparentNoCull,
            RENDER_PIPELINE_CharBlocksWithDepthTest,
            RENDER_PIPELINE_OrthoPetscii,
            RENDER_PIPELINE_OrthoPetsciiOverlay,
            RENDER_PIPELINE_OrthoVoxels,
            RENDER_PIPELINE_LinesWithNoDepthTest,
            RENDER_PIPELINE_CLEAR,
            RENDER_PIPELINE_SWAP,
            UPDATE_TOTAL,
            UPDATE_CNT_FRAME,
            UPDATE_SETTINGS,
            UPDATE_PHYSICS,
            UPDATE_LANDSCAPE,
            UPDATE_UI,
            UPDATE_CAM,
            UPDATE_COLLISION,
            UPDATE_ITEMS,
            JOB_ADDED_RENDER,
            JOB_ADDED_UPDATE,
        }

        public void Begin(Section sectionName)
        {
            var name = sectionName.ToString();
            if (!component.PerformanceCounters.ContainsKey(name))
            {
                component.PerformanceCounters[name] = new PerformanceCounter(name);
            }

            component.PerformanceCounters[name].Stopwatch.Start();
        }

        public void Profile(Section sectionName, Action action)
        {
            Begin(sectionName);
            action();
            End(sectionName);
        }

        public void End(Section sectionName)
        {
            var name = sectionName.ToString();
            component.PerformanceCounters[name].Stopwatch.Stop();
        }

        public void ClearAll()
        {
            component.PerformanceCounters.Clear();
        }

        public long QueryTimer(Section sectionName)
        {
            var name = sectionName.ToString();
            if (!component.PerformanceCounters.ContainsKey(name))
            {
                return 0;
            }

            return component.PerformanceCounters[name].Stopwatch.ElapsedMilliseconds;
        }

        public long QueryCount(Section sectionName)
        {
            var name = sectionName.ToString();
            if (!component.PerformanceCounters.ContainsKey(name))
            {
                return 0;
            }

            return component.PerformanceCounters[name].Count;
        }

        public void Increase(Section sectionName, int count = 1)
        {
            var name = sectionName.ToString();
            if (!component.PerformanceCounters.ContainsKey(name))
            {
                component.PerformanceCounters[name] = new PerformanceCounter(name);
            }

            component.PerformanceCounters[name].Count += count;
        }
    }
}
