namespace ConsoleApp1.AllSystems.Updating.Services
{
    using OpenTK.Windowing.GraphicsLibraryFramework;

    public static class KeyboardService
    {
        public static bool KeyPressed(KeyboardState keyboardState, KeyboardState keyboardStatePrev, Keys key)
        {
            return keyboardStatePrev.IsKeyDown(key) && !keyboardState.IsKeyDown(key);
        }
    }
}
