namespace ConsoleApp1.Components
{
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.Entities;

    public class ItemRenderComponent : DisposableComponentBase
    {
        public Entity Id { get; set; }

        public VAOWrapper Vao { get; } = new VAOWrapper();

        protected override void DisposeMe()
        {
            Vao.Delete();
        }
    }
}