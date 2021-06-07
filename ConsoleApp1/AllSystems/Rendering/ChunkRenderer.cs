namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Rendering.Services;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    public class ChunkRenderer
    {
        private const int OpaqueIndex = 0;
        private const int TransparentIndex = 1;
        private const int TransparentNoCullIndex = 2;

        private bool disposed;
        private JobPool jobPool;
        private LandscapeComponent landscape;
        private ChunkService chunkService = new ChunkService();
        private Chunk entity;

        private RenderPipeline pipeline;
        private VAOWrapper vaoOpaque = new VAOWrapper();
        private VAOWrapper vaoTransparent = new VAOWrapper();
        private VAOWrapper vaoTransparentNoCull = new VAOWrapper();
        private Guid? jobToken = null;
        private bool builtAtLeastOnce = false;

        public ChunkRenderer(JobPool jobPool, LandscapeComponent landscape, Chunk entity, RenderPipeline pipeline)
        {
            this.jobPool = jobPool;
            this.landscape = landscape;
            this.entity = entity;
            this.pipeline = pipeline;
        }

        ~ChunkRenderer()
        {
            Dispose(false);
        }

        public bool NeedRebuild
        {
            get
            {
                return entity.Dirty || (!entity.IsEmpty && !builtAtLeastOnce);
            }
        }

        public bool Build()
        {
            if (jobPool.IsProcessing(jobToken))
            {
                return false;
            }

            chunkService.Bind(landscape, entity);
            var chunkCacheService = new ChunkCacheService();
            chunkCacheService.Load(chunkService, entity);
            chunkService.Unbind();
            jobToken = jobPool.AddRenderJob(BuildVertexList, chunkCacheService);
            entity.Dirty = false;
            builtAtLeastOnce = true;
            return true;
        }

        public void Render(Matrix4 view, Matrix4 proj)
        {
            GetJobResultIfAny();
            Matrix4 translate = Matrix4.CreateTranslation(entity.Position);
            Matrix4 model = translate;
            Matrix4 mvp = model * view * proj;
            if (vaoOpaque.Created)
            {
                pipeline.Enqueue(DrawPipeline.BlocksWithDepthTest, new PipelineCommand(mvp, vaoOpaque));
            }

            if (vaoTransparent.Created)
            {
                pipeline.Enqueue(DrawPipeline.TransparentWithDepthTest, new PipelineCommand(mvp, vaoTransparent));
            }

            if (vaoTransparentNoCull.Created)
            {
                pipeline.Enqueue(DrawPipeline.TransparentNoCull, new PipelineCommand(mvp, vaoTransparentNoCull));
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
                vaoOpaque.Delete();
                vaoTransparent.Delete();
                vaoTransparentNoCull.Delete();
            }

            disposed = true;
        }

        private void GetJobResultIfAny()
        {
            if (!jobPool.PendingResult(jobToken))
            {
                return;
            }

            var newVertices = jobPool.GetResult<VertexBuffer[]>(jobToken.Value);
            jobToken = null;
            var shader = pipeline.VoxelShader;
            if (newVertices[0].Size > 0)
            {
                vaoOpaque
                   .CreateAndBind(newVertices[0])
                   .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aTexCoord", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aSkyLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ApplyAndUnbind();
            }

            if (newVertices[1].Size > 0)
            {
                vaoTransparent
                   .CreateAndBind(newVertices[1])
                   .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aTexCoord", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aSkyLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ApplyAndUnbind();
            }

            if (newVertices[2].Size > 0)
            {
                vaoTransparentNoCull
                   .CreateAndBind(newVertices[2])
                   .ConfigAddVertices(shader, "aPos", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aTexCoord", 3, VertexAttribPointerType.Float)
                   .ConfigAddVertices(shader, "aCol", 3, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ConfigAddVertices(shader, "aSkyLight", 1, VertexAttribPointerType.UnsignedByte)
                   .ApplyAndUnbind();
            }
        }

        private object BuildVertexList(object arguments)
        {
            ChunkCacheService chunkCacheService = (ChunkCacheService)arguments;
            chunkCacheService.CopyMain();

            var allVertices = BuildVertices(3, chunkCacheService, (block) =>
            {
                if (!IsPlant(block))
                {
                    return OpaqueIndex;
                }
                else if (IsPlant(block))
                {
                    return TransparentNoCullIndex;
                }

                return TransparentIndex;
            });

            return allVertices;

            bool IsPlant(Block block)
            {
                return block.Id == BlockId.TallGrass
                    || block.Id == BlockId.FlowerDandelion
                    || block.Id == BlockId.FlowerRose;
            }
        }

        private VertexBuffer[] BuildVertices(int arrayCount, ChunkCacheService chunkCacheService, Func<Block, int> getVertexArrayIndex)
        {
            VertexBuffer[] vertexBuffer = new VertexBuffer[arrayCount];
            for (int i = 0; i < arrayCount; i++)
            {
                vertexBuffer[i] = new VertexBuffer();
            }

            for (var x = 0; x < Chunk.ChunkSize; x++)
            {
                for (var y = 0; y < Chunk.ChunkSize; y++)
                {
                    for (var z = 0; z < Chunk.ChunkSize; z++)
                    {
                        Vector3i localPos = new Vector3i(x, y, z);
                        var thisBlock = BlockRepository.Get(chunkCacheService.GetLocalVoxelAt(localPos));
                        var arrayIndex = getVertexArrayIndex(thisBlock);
                        if (!thisBlock.Visible || (arrayIndex < 0 || arrayIndex >= arrayCount))
                        {
                            continue;
                        }

                        var res = BlockTextureRepository.Get(thisBlock.Id);
                        vertexBuffer[arrayIndex].AddBuffer(res.MeshBuilder.Build(chunkCacheService, localPos, thisBlock, res));
                    }
                }
            }

            return vertexBuffer;
        }
    }
}