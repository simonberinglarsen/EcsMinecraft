namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class InventoryRenderer
    {
        private bool disposed;
        private VAOWrapper[] vaos = new VAOWrapper[8];
        private RenderPipeline pipeline;
        private InventoryComponent inventory;

        public InventoryRenderer(RenderPipeline pipeline, InventoryComponent inventory)
        {
            this.pipeline = pipeline;
            this.inventory = inventory;
            for (int i = 0; i < vaos.Length; i++)
            {
                vaos[i] = new VAOWrapper();
            }
        }

        ~InventoryRenderer()
        {
            Dispose(false);
        }

        public void Build()
        {
            const float s1 = 1f;
            const float s2 = .6f;
            const float s3 = .3f;

            for (int i = 0; i < 8; i++)
            {
                VertexBuffer newVertices = new VertexBuffer();
                var slot = inventory.Slots[i];
                if (slot.BlockId == BlockId.Void)
                {
                    vaos[i].Delete();
                    continue;
                }

                newVertices.AddBuffer(CubeVertices.FromBlockId(Vector3.Zero, slot.BlockId, s1, Face.Up));
                newVertices.AddBuffer(CubeVertices.FromBlockId(Vector3.Zero, slot.BlockId, s2, Face.South));
                newVertices.AddBuffer(CubeVertices.FromBlockId(Vector3.Zero, slot.BlockId, s3, Face.East));
                var shader = pipeline.VoxelShader;
                vaos[i]
                    .CreateAndBind(newVertices)
                    .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                    .ConfigAddVertices(shader, "aTexCoord", 3, VertexAttribPointerType.Float)
                    .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.UnsignedByte)
                    .ConfigAddVertices(shader, "aLight", 1, VertexAttribPointerType.UnsignedByte)
                    .ConfigAddVertices(shader, "aSkyLight", 1, VertexAttribPointerType.UnsignedByte)
                    .ApplyAndUnbind();
            }
        }

        public void Render(Matrix4 view, Matrix4 proj)
        {
            if (inventory.Dirty)
            {
                Build();
                inventory.Dirty = false;
            }

            var game = Game.Instance;
            Matrix4 scale = Matrix4.CreateScale(40f);
            float rot45 = (float)(Math.PI / 4f);
            float rot22 = (float)(Math.PI / 8f);
            Matrix4 rotate = Matrix4.CreateRotationY(-rot45) * Matrix4.CreateRotationX(rot22);
            Matrix4 sr = scale * rotate;

            float charSize = 32f;
            float slotChars = 3;
            float slotSize = slotChars * charSize;
            Vector3 bottomCenter = new Vector3(game.Size.X / 2f, -game.Size.Y + (2 * slotSize / 5f), 0);
            Vector3 firstSlotCenter = bottomCenter + new Vector3(-3.5f * slotSize, 0, 0);
            for (int i = 0; i < vaos.Length; i++)
            {
                if (!vaos[i].Created)
                {
                    continue;
                }

                Vector3 ofs = firstSlotCenter + new Vector3(i * slotSize, 0, -100);
                Matrix4 translate = Matrix4.CreateTranslation(ofs);
                Matrix4 model = sr * translate;
                Matrix4 mvp = model * view * proj;
                pipeline.Enqueue(DrawPipeline.OrthoVoxels, new PipelineCommand(mvp, vaos[i]));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                for (int i = 0; i < vaos.Length; i++)
                {
                    vaos[i].Delete();
                }
            }

            disposed = true;
        }
    }
}