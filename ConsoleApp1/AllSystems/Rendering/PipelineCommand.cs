namespace ConsoleApp1.AllSystems.Rendering
{
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using OpenTK.Mathematics;

    public class PipelineCommand
    {
        public PipelineCommand(Matrix4 mvp, VAOWrapper vao)
        {
            Mvp = mvp;
            Vao = vao;
        }

        public Matrix4 Mvp { get; }

        public VAOWrapper Vao { get; }
    }
}