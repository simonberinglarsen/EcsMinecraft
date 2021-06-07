namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class ItemRenderSystem
    {
        private RenderPipeline pipeline;

        public ItemRenderSystem(RenderPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public void Run(float alpha, Context context)
        {
            var itemRenderer = context.ItemRenderer;
            var item = context.Item;
            var physics = context.Physics;
            if (!itemRenderer.Vao.Created)
            {
                var buffer = new VertexBuffer();
                var p = new Vector3(-.5f, -.5f, -.5f);
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 1f, Face.Up));
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 0.5f, Face.Down));
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 0.9f, Face.North));
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 0.7f, Face.East));
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 0.9f, Face.South));
                buffer.AddBuffer(CubeVertices.FromBlockId(p, item.BlockId, 0.7f, Face.West));
                var shader = pipeline.VoxelShader;
                itemRenderer.Vao.CreateAndBind(buffer)
                   .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aTexCoord", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aSkyLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ApplyAndUnbind();
            }

            Vector3 pos = physics.PositionPrev + ((physics.Position - physics.PositionPrev) * alpha);
            var time = item.Age + item.Random;
            var ofsY = Math.Sin(time / 4f);
            var ofsYPrev = Math.Sin((time - 1) / 4f);
            var ofsYNow = (float)(ofsYPrev + ((ofsY - ofsYPrev) * alpha));
            var ofs = new Vector3(0, physics.Height + (ofsYNow * 0.1f), 0);
            var rotation = time / 8f;
            var rotationPrev = (time - 1) / 8f;

            var rot = rotationPrev + ((rotation - rotationPrev) * alpha);
            Matrix4 rotate = Matrix4.CreateRotationY(rot) * Matrix4.CreateRotationX(rot);
            Matrix4 translate = Matrix4.CreateTranslation(pos + ofs);
            Matrix4 scale = Matrix4.CreateScale(physics.Width);
            Matrix4 model = scale * rotate * translate;
            Matrix4 mvp = model * context.ViewProj;

            pipeline.Enqueue(DrawPipeline.BlocksWithDepthTest, new PipelineCommand(mvp, itemRenderer.Vao));
            if (context.Item.Count > 1)
            {
                rot += (float)(Math.PI / 5f);
                ofs += new Vector3(0.25f, 0, 0);
                rotate = Matrix4.CreateRotationY(rot) * Matrix4.CreateRotationX(rot);
                translate = Matrix4.CreateTranslation(pos + ofs);
                model = scale * rotate * translate;
                mvp = model * context.ViewProj;
                pipeline.Enqueue(DrawPipeline.BlocksWithDepthTest, new PipelineCommand(mvp, itemRenderer.Vao));
            }

            if (context.Item.Count > 2)
            {
                rot += (float)(Math.PI / 5f);
                ofs += new Vector3(-0.15f, 0, 0.15f);
                rotate = Matrix4.CreateRotationY(rot) * Matrix4.CreateRotationX(rot);
                translate = Matrix4.CreateTranslation(pos + ofs);
                model = scale * rotate * translate;
                mvp = model * context.ViewProj;
                pipeline.Enqueue(DrawPipeline.BlocksWithDepthTest, new PipelineCommand(mvp, itemRenderer.Vao));
            }
        }

        public class Context
        {
            public Matrix4 ViewProj { get; set; }

            public ItemComponent Item { get; set; }

            public PhysicsComponent Physics { get; set; }

            public ItemRenderComponent ItemRenderer { get; set; }
        }
    }
}