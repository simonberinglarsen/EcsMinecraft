namespace ConsoleApp1.AllSystems.Rendering.Services
{
    using System;
    using OpenTK.Mathematics;

    public class BlockFaceResource
    {
        public Vector3 Color { get; set; }

        public int TextureIndex { get; set; }

        public Vector2[] TextureCoords { get; set; }
    }
}
