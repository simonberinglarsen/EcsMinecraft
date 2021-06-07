namespace ConsoleApp1
{
    using System.Diagnostics;
    using ConsoleApp1.AllSystems;
    using ConsoleApp1.AllSystems.Rendering;
    using ConsoleApp1.AllSystems.Updating;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class Game : GameWindow
    {
        public static readonly Game Instance = new Game();
        private const double FixedUpdateTime = 0.050d;
        private Stopwatch timer = new Stopwatch();
        private double lastVisit = 0;
        private double accumulator = 0;
        private RenderSystem renderSystem;
        private UpdateSystem updateSystem;

        private Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            // VSync = OpenTK.Windowing.Common.VSyncMode.On;
            Size = new Vector2i(974, 674);
            Load += Game_Load;
            Resize += Game_Resize;
            Unload += Game_Unload;
            MousePosition = new Vector2(Size.X / 2, Size.Y / 2);
        }

        public override void Run()
        {
            timer.Start();
            Game_Load();
            Game_Resize(default(OpenTK.Windowing.Common.ResizeEventArgs));
            while (true)
            {
                ProcessEvents();
                if (Exists && !IsExiting)
                {
                    InnerGameloop();
                }
                else
                {
                    return;
                }
            }
        }

        private void Game_Unload()
        {
            renderSystem.Dispose();
        }

        private void InnerGameloop()
        {
            double thisVisit = timer.Elapsed.TotalSeconds;
            double elapsedTime = thisVisit - lastVisit;
            if (elapsedTime > 0.25)
            {
                elapsedTime = 0.25;
            }

            accumulator += elapsedTime;
            while (accumulator >= FixedUpdateTime)
            {
                Update();
                accumulator -= FixedUpdateTime;
            }

            float alpha = (float)(accumulator / FixedUpdateTime);
            Render(alpha);
            lastVisit = thisVisit;
        }

        private void Update()
        {
            updateSystem.Run(this);
        }

        private void Render(float alpha)
        {
            renderSystem.Run(alpha);
        }

        private void Game_Resize(OpenTK.Windowing.Common.ResizeEventArgs obj)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            EntityDatabase.SettingsComponent.Size = Size;
        }

        private void Game_Load()
        {
            EntityDatabase.InputComponent.KeyboardState = EntityDatabase.InputComponent.KeyboardStatePrev = KeyboardState.GetSnapshot();
            EntityDatabase.InputComponent.MouseState = EntityDatabase.InputComponent.MouseStatePrev = MouseState.GetSnapshot();

            var jobPool = new JobPool(EntityDatabase.PerformanceComponent);
            updateSystem = new UpdateSystem(jobPool, EntityDatabase.PerformanceComponent);
            renderSystem = new RenderSystem(jobPool, EntityDatabase.PerformanceComponent);

            EntityDatabase.CreateUI();
            EntityDatabase.CreatePlayer();
        }
    }
}