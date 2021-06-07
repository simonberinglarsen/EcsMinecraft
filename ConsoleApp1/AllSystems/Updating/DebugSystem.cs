namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class DebugSystem
    {
        private ScreenService service = new ScreenService();
        private JobPool jobPool;
        private PerformanceService performance;
        private Dictionary<int, Action> pages;
        private LandscapeComponent landscape;

        public DebugSystem(JobPool jobPool, PerformanceComponent performance)
        {
            this.jobPool = jobPool;
            this.performance = new PerformanceService(performance);
            this.pages = new Dictionary<int, Action>
            {
                { 1, ShowPage1 },
                { 2, ShowPage2 },
                { 3, ShowPage3 },
            };
            if (this.pages.Count != SettingsComponent.DebugPages)
            {
                throw new Exception("Please update debug pages property!");
            }
        }

        public void Run(LandscapeComponent landscape, SettingsComponent settings, ScreenComponent screen)
        {
            this.landscape = landscape;
            var showDebug = settings.ShowDebugPage != 0;
            if (!showDebug)
            {
                screen.Visible = false;
                return;
            }

            bool changed = screen.Visible != showDebug;
            screen.Visible = true;
            if (settings.GameTick % 20 == 0 || changed)
            {
                service.Bind(screen);
                service.Clear();
                pages[settings.ShowDebugPage]();
            }
        }

        private void ShowPage1()
        {
            int line = 0;
            LogLineH(line++, "render");
            LogLineC(line++, $"fps:{performance.QueryCount(PerformanceService.Section.RENDER_FRAME_COUNT)}");
            LogLineT(line++, $"render-total:{performance.QueryTimer(PerformanceService.Section.RENDER_TOTAL)}");
            LogLineT(line++, $"world-space:{performance.QueryTimer(PerformanceService.Section.RENDER_WORLD_SPACE)}");
            LogLineT(line++, $"landscape:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE)}");
            LogLineT(line++, $"-init:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_INIT_INACTIVE)}");
            LogLineT(line++, $"-render:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS)}");
            LogLineT(line++, $"--column:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS)}");
            LogLineT(line++, $"---visible:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_VISIBLE)}");
            LogLineT(line++, $"---render:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_RENDER_COLS_CHUNKS_RENDER)}");
            LogLineT(line++, $"-dispose:{performance.QueryTimer(PerformanceService.Section.RENDER_LANDSCAPE_DISPOSE_INACTIVE)}");
            LogLineT(line++, $"ortho:{performance.QueryTimer(PerformanceService.Section.RENDER_ORTHO)}");
            LogLineT(line++, $"pipeline:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE)}");
        }

        private void ShowPage2()
        {
            int line = 0;
            LogLineH(line++, "Pipeline");
            LogLineC(line++, $"fps:{performance.QueryCount(PerformanceService.Section.RENDER_FRAME_COUNT)}");
            LogLineT(line++, $"blocks:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_BlocksWithDepthTest)}");
            LogLineT(line++, $"block-char:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_CharBlocksWithDepthTest)}");
            LogLineT(line++, $"block-ortho:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_OrthoVoxels)}");
            LogLineT(line++, $"simple:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_SimpleWithDepthTest)}");
            LogLineT(line++, $"lines:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_LinesWithDepthTest)}");
            LogLineT(line++, $"lines-nodepth:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_LinesWithNoDepthTest)}");
            LogLineT(line++, $"trans:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_TransparentWithDepthTest)}");
            LogLineT(line++, $"trans-no-cull:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_TransparentNoCull)}");
            LogLineT(line++, $"petscii:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_OrthoPetscii)}");
            LogLineT(line++, $"petscii-overlay:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_OrthoPetsciiOverlay)}");
            LogLineT(line++, $"clear:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_CLEAR)}");
            LogLineT(line++, $"swap:{performance.QueryTimer(PerformanceService.Section.RENDER_PIPELINE_SWAP)}");
        }

        private void ShowPage3()
        {
            int line = 0;
            LogLineH(line++, "Updating");
            LogLineC(line++, $"fps:{performance.QueryCount(PerformanceService.Section.UPDATE_CNT_FRAME)}");
            LogLineT(line++, $"total:{performance.QueryTimer(PerformanceService.Section.UPDATE_TOTAL)}");
            LogLineT(line++, $"settings:{performance.QueryTimer(PerformanceService.Section.UPDATE_SETTINGS)}");
            LogLineT(line++, $"physics:{performance.QueryTimer(PerformanceService.Section.UPDATE_PHYSICS)}");
            LogLineT(line++, $"player:{performance.QueryTimer(PerformanceService.Section.UPDATE_ITEMS)}");
            LogLineT(line++, $"landscape:{performance.QueryTimer(PerformanceService.Section.UPDATE_LANDSCAPE)}");
            LogLineT(line++, $"ui:{performance.QueryTimer(PerformanceService.Section.UPDATE_UI)}");
            LogLineT(line++, $"cam:{performance.QueryTimer(PerformanceService.Section.UPDATE_CAM)}");
            LogLineT(line++, $"collision:{performance.QueryTimer(PerformanceService.Section.UPDATE_COLLISION)}");
            LogLineT(line++, $"items:{performance.QueryTimer(PerformanceService.Section.UPDATE_ITEMS)}");
        }

        private void LogLineH(int lineNo, string msg)
        {
            LogLine(lineNo, msg, Color.Red, Color.Red);
        }

        private void LogLineC(int lineNo, string msg)
        {
            LogLine(lineNo, msg.Replace(":", ":#"), Color.White, Color.LightPeach);
        }

        private void LogLineT(int lineNo, string msg)
        {
            LogLine(lineNo, msg, Color.LightGrey, Color.Yellow);
        }

        private void LogLine(int lineNo, string msg, Color c1, Color c2)
        {
            int part2 = msg.IndexOf(":") + 1;
            service.Print(0, lineNo, msg.Substring(0, part2), c1);
            service.Print(part2, lineNo, msg.Substring(part2).PadRight(30 - msg.Length), c2);
        }
    }
}