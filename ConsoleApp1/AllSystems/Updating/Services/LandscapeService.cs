namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class LandscapeService : IWorldAccess
    {
        private LandscapeComponent entity;
        private LightAlgorithm lightAlg;

        public LandscapeService()
        {
            lightAlg = new LightAlgorithm(this);
        }

        public void Bind(LandscapeComponent entity)
        {
            this.entity = entity;
        }

        public void Unbind()
        {
            Bind(null);
        }

        public ushort GetGlobalVoxelAt(Vector3i voxelPos)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return BlockId.Void;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            return col[index].Voxels[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)];
        }

        public Vector3i ToVector3i(Vector3 pos)
        {
            return new Vector3i(
                (int)Math.Floor(pos.X),
                (int)Math.Floor(pos.Y),
                (int)Math.Floor(pos.Z));
        }

        public void LightFlow(Vector3i p)
        {
            SetGlobalVoxelWithLightAt(p, BlockId.Lava);
        }

        public void SetGlobalVoxelWithLightAt(Vector3i p, ushort v)
        {
            lightAlg.Propagate(p, () =>
            {
                SetGlobalVoxelAt(p, v);
            });
        }

        public void SetGlobalVoxelAt(Vector3i voxelPos, ushort v)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            var offset = localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared);
            col[index].Voxels[offset] = v;
            MarkChunksAsDirty(col, index, localPos, voxelPos);
        }

        public byte GetGlobalLightAt(Vector3i voxelPos)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return 0;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            var v = col[index].Voxels[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)];
            var block = BlockRepository.Get(v);
            if (!block.LightPasses)
            {
                return block.EmittedLight;
            }

            var l = col[index].Lights[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)];
            return Math.Max(block.EmittedLight, l);
        }

        public AxisAlignedBoundingBox[] GetSurroundingBoundingBoxes(Vector3 min, Vector3 max)
        {
            List<AxisAlignedBoundingBox> all = new List<AxisAlignedBoundingBox>();
            Vector3i minVoxel = ToVector3i(min) - new Vector3i(1, 1, 1);
            Vector3i maxVoxel = ToVector3i(max) + new Vector3i(1, 1, 1);
            for (int x = minVoxel.X; x <= maxVoxel.X; x++)
            {
                for (int y = minVoxel.Y; y <= maxVoxel.Y; y++)
                {
                    for (int z = minVoxel.Z; z <= maxVoxel.Z; z++)
                    {
                        int v = GetGlobalVoxelAt(new Vector3i(x, y, z));
                        var aabb = BlockRepository.Get(v).BoundingBox;
                        if (aabb == null || !BlockRepository.Get(v).Rigid)
                        {
                            continue;
                        }

                        aabb.MoveTo(new Vector3(x, y, z));
                        all.Add(aabb);
                    }
                }
            }

            return all.ToArray();
        }

        public void SetGlobalSkyLightAt(Vector3i voxelPos, byte l)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            col[index].SkyLights[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)] = l;
            MarkChunksAsDirty(col, index, localPos, voxelPos);
        }

        public byte GetGlobalSkyLightAt(Vector3i voxelPos)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return 0;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            return col[index].SkyLights[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)];
        }

        public void SetGlobalLightAt(Vector3i voxelPos, byte l)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(voxelPos);
            if (!entity.ChunkColumns.ContainsKey(colPos) || voxelPos.Y < 0 || voxelPos.Y >= 256)
            {
                return;
            }

            var col = entity.ChunkColumns[colPos].Chunks;
            var index = (int)voxelPos.Y / 16;
            var localPos = voxelPos - (new Vector3i(colPos.X, index, colPos.Y) * 16);
            col[index].Lights[localPos.X + (localPos.Y * Chunk.ChunkSize) + (localPos.Z * Chunk.ChunkSizeSquared)] = l;
            MarkChunksAsDirty(col, index, localPos, voxelPos);
        }

        int IWorldAccess.GetLight(Vector3i p)
        {
            return GetGlobalLightAt(p);
        }

        void IWorldAccess.SetLight(Vector3i p, int l)
        {
            SetGlobalLightAt(p, (byte)l);
        }

        int IWorldAccess.GetSkyLight(Vector3i p)
        {
            return GetGlobalSkyLightAt(p);
        }

        void IWorldAccess.SetSkyLight(Vector3i p, int l)
        {
            SetGlobalSkyLightAt(p, (byte)l);
        }

        bool IWorldAccess.NoLightPasses(Vector3i p)
        {
            int v = GetGlobalVoxelAt(p);
            return !BlockRepository.Get(v).LightPasses;
        }

        bool IWorldAccess.OutOfBounds(Vector3i p)
        {
            return false;
        }

        public Vector2i ColumnPosFromVoxelPos(Vector3i pos)
        {
            return new Vector2i(
                pos.X < 0 ? ((pos.X + 1) / 16) - 1 : pos.X / 16,
                pos.Z < 0 ? ((pos.Z + 1) / 16) - 1 : pos.Z / 16);
        }

        private void MarkChunksAsDirty(Chunk[] col, int index, Vector3i local, Vector3i globalPos)
        {
            col[index].Dirty = true;
            if (!IsBorderVoxel(local))
            {
                return;
            }

            if ((local.Y == Chunk.ChunkSize - 1) && (index < LandscapeComponent.LandscapeHeightInChunks - 1))
            {
                col[index + 1].Dirty = true;
            }
            else if ((local.Y == 0) && (index > 0))
            {
                col[index - 1].Dirty = true;
            }

            Action<Vector3i> dirtyIfNotNull = (offset) =>
            {
                var col = ColumnFromVoxelPos(globalPos + offset);
                if (col != null)
                {
                    col[index].Dirty = true;
                }
            };

            if (local.X == Chunk.ChunkSize - 1)
            {
                dirtyIfNotNull(new Vector3i(1, 0, 0));
            }
            else if (local.X == 0)
            {
                dirtyIfNotNull(new Vector3i(-1, 0, 0));
            }

            if (local.Z == Chunk.ChunkSize - 1)
            {
                dirtyIfNotNull(new Vector3i(0, 0, 1));
            }
            else if (local.Z == 0)
            {
                dirtyIfNotNull(new Vector3i(0, 0, -1));
            }
        }

        private Chunk[] ColumnFromVoxelPos(Vector3i globalPos)
        {
            Vector2i colPos = ColumnPosFromVoxelPos(globalPos);
            if (!entity.ChunkColumns.ContainsKey(colPos))
            {
                return null;
            }

            return entity.ChunkColumns[colPos].Chunks;
        }

        private bool IsBorderVoxel(Vector3i local)
        {
            return local.X == 0 || local.X == Chunk.ChunkSize - 1
                || local.Y == 0 || local.Y == Chunk.ChunkSize - 1
                || local.Z == 0 || local.Z == Chunk.ChunkSize - 1;
        }
    }
}