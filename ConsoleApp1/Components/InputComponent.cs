namespace ConsoleApp1.Components
{
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public class InputComponent
    {
        public KeyboardState KeyboardState { get; set; }

        public KeyboardState KeyboardStatePrev { get; set; }

        public MouseState MouseStatePrev { get; set; }

        public MouseState MouseState { get; set; }
    }
}