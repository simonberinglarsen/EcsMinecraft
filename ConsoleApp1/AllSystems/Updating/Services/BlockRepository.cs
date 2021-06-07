namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System.Collections.Generic;
    using ConsoleApp1.Components;

    public static class BlockRepository
    {
        static BlockRepository()
        {
            Add(new BlockBuilder().Id(BlockId.Void).Invisible().LightPasses().NoAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Stone).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Grass).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Dirt).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Cobblestone).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Planks).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.GoldOre).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.IronOre).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.CoalOre).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Log).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Leaves).FullAABB().NotOpaque().LightPasses().CannotHideNeighborFace().Build());
            Add(new BlockBuilder().Id(BlockId.GoldBlock).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Glass).FullAABB().NotOpaque().LightPasses().Build());
            Add(new BlockBuilder().Id(BlockId.Water).FullAABB().NotOpaque().LightPasses().Build());
            Add(new BlockBuilder().Id(BlockId.Lava).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Sand).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.PC).FullAABB().Build());
            Add(new BlockBuilder().Id(BlockId.Torch).MaxLight().NotRigid().LightPasses().AABB(new AxisAlignedBoundingBox
            {
                MinX = 7f / 16f,
                MinY = 0,
                MinZ = 7f / 16f,
                MaxX = 9f / 16f,
                MaxY = 10 / 16f,
                MaxZ = 9f / 16f,
            }).Build());
            Add(new BlockBuilder().Id(BlockId.TallGrass).FullAABB().NotRigid().NotOpaque().LightPasses().CannotHideNeighborFace().Build());
            Add(new BlockBuilder().Id(BlockId.FlowerDandelion).FullAABB().NotRigid().NotOpaque().LightPasses().CannotHideNeighborFace().Build());
            Add(new BlockBuilder().Id(BlockId.FlowerRose).FullAABB().NotRigid().NotOpaque().LightPasses().CannotHideNeighborFace().Build());
        }

        public static Dictionary<int, Block> BlockMap { get; } = new Dictionary<int, Block>();

        public static Dictionary<string, Block> NameMap { get; } = new Dictionary<string, Block>();

        public static void Add(Block b)
        {
            BlockMap[b.Id] = b;
        }

        public static Block Get(int v)
        {
            return BlockMap[v];
        }

        public static Block Get(string name)
        {
            return NameMap[name];
        }
    }
}