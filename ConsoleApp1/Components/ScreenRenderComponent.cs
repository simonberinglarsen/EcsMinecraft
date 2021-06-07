namespace ConsoleApp1.Components
{
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ScreenRenderComponent
    {
        public Entity Id { get; set; }

        public VAOWrapper Vao { get; } = new VAOWrapper();
    }
}