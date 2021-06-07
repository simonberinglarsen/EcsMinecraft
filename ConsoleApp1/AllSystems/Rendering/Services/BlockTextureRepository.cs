namespace ConsoleApp1.AllSystems.Rendering.Services
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Updating.Services;
    using OpenTK.Mathematics;

    public static class BlockTextureRepository
    {
        private static Dictionary<ushort, BlockResource> voxelToResourceMap = new Dictionary<ushort, BlockResource>();

        public static Texture2DArray BlockTextures { get; } = new Texture2DArray();

        public static void Load()
        {
            Vector3 green = new Vector3(0.7f, 1, 0.4f);
            Vector3 darkGreen = new Vector3(0.2f, 0.6f, 0.2f);
            Add(new BlockResourceBuilder(BlockId.Stone).WithAllTex(FromFile("stone")).Build());
            Add(new BlockResourceBuilder(BlockId.Grass)
                .WithUpTex(FromFile("grass_top"))
                .WithDownTex(FromFile("dirt"))
                .WithSideTex(FromFile("grass_side"))
                .WithUpColor(green)
                .Build());
            Add(new BlockResourceBuilder(BlockId.TallGrass)
                .WithPlantMesh()
                .WithAllTex(FromFile("tallgrass"))
                .WithAllColors(green)
                .Build());
            Add(new BlockResourceBuilder(BlockId.FlowerDandelion)
                .WithPlantMesh()
                .WithAllTex(FromFile("flower_dandelion"))
                .Build());
            Add(new BlockResourceBuilder(BlockId.FlowerRose)
                .WithPlantMesh()
                .WithAllTex(FromFile("flower_rose"))
                .Build());
            Add(new BlockResourceBuilder(BlockId.Dirt).WithAllTex(FromFile("dirt")).Build());
            Add(new BlockResourceBuilder(BlockId.Cobblestone).WithAllTex(FromFile("cobblestone")).Build());
            Add(new BlockResourceBuilder(BlockId.Planks).WithAllTex(FromFile("planks_oak")).Build());
            Add(new BlockResourceBuilder(BlockId.GoldOre).WithAllTex(FromFile("gold_ore")).Build());
            Add(new BlockResourceBuilder(BlockId.IronOre).WithAllTex(FromFile("iron_ore")).Build());
            Add(new BlockResourceBuilder(BlockId.CoalOre).WithAllTex(FromFile("coal_ore")).Build());
            Add(new BlockResourceBuilder(BlockId.Log)
                .WithUpTex(FromFile("log_oak_top"))
                .WithDownTex(FromFile("log_oak_top"))
                .WithSideTex(FromFile("log_oak"))
                .Build());
            Add(new BlockResourceBuilder(BlockId.Leaves)
                .WithAllTex(FromFile("leaves_oak"))
                .WithAllColors(darkGreen)
                .Build());
            Add(new BlockResourceBuilder(BlockId.GoldBlock).WithAllTex(FromFile("gold_block")).Build());
            Add(new BlockResourceBuilder(BlockId.Glass).WithAllTex(FromFile("glass")).Build());
            Add(new BlockResourceBuilder(BlockId.Water).WithAllTex(FromFile("glass_purple")).Build());
            Add(new BlockResourceBuilder(BlockId.Lava).WithAllTex(FromFile("glass_purple")).Build());
            Add(new BlockResourceBuilder(BlockId.Sand).WithAllTex(FromFile("sand")).Build());
            Add(new BlockResourceBuilder(BlockId.PC)
                .WithUpTex(FromFile("pc_top"))
                .WithDownTex(FromFile("pc_top"))
                .WithSideTex(FromFile("pc"))
                .Build());
            Add(new BlockResourceBuilder(BlockId.Torch)
                .WithAllTex(FromFile("torch_on"))
                .WithTextureCoordsSides(new Vector2[] { new Vector2(7 / 16f, 0), new Vector2(9 / 16f, 0), new Vector2(9 / 16f, 10 / 16f), new Vector2(7 / 16f, 10 / 16f) })
                .WithTextureCoordsUp(new Vector2[] { new Vector2(7 / 16f, 8 / 16f), new Vector2(9 / 16f, 8 / 16f), new Vector2(9 / 16f, 10 / 16f), new Vector2(7 / 16f, 10 / 16f) })
                .WithTextureCoordsDown(new Vector2[] { new Vector2(7 / 16f, 0), new Vector2(9 / 16f, 0), new Vector2(9 / 16f, 2 / 16f), new Vector2(7 / 16f, 2 / 16f) })
                .Build());
            BlockTextures.Compile();
        }

        public static BlockResource Get(ushort id)
        {
            return voxelToResourceMap[id];
        }

        private static void Add(BlockResource resource)
        {
            voxelToResourceMap[resource.Id] = resource;
        }

        private static int FromFile(string filename)
        {
            var asPng = $"assets/{filename}.png";
            return BlockTextures.Save(asPng);
        }
    }
}
