namespace ConsoleApp1.AllSystems.Rendering.Services
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering.MeshBuilders;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class BlockResourceBuilder
    {
        private static IVoxelMeshBuilder cubeMesh = new CubeMeshBuilder();
        private static IVoxelMeshBuilder plantMesh = new PlantMeshBuilder();
        private ushort id;
        private BlockFaceResource up;
        private BlockFaceResource down;
        private BlockFaceResource north;
        private BlockFaceResource south;
        private BlockFaceResource east;
        private BlockFaceResource west;
        private IVoxelMeshBuilder meshBuilder;

        public BlockResourceBuilder(ushort id)
        {
            this.id = id;
            up = CreateDefaultFace();
            down = CreateDefaultFace();
            north = CreateDefaultFace();
            south = CreateDefaultFace();
            east = CreateDefaultFace();
            west = CreateDefaultFace();
            meshBuilder = cubeMesh;
        }

        public BlockResourceBuilder WithPlantMesh()
        {
            meshBuilder = plantMesh;
            return this;
        }

        public BlockResourceBuilder WithTextureCoordsSides(Vector2[] coords)
        {
            if (coords.Length != 4)
            {
                throw new Exception("expected 4 texture coords");
            }

            east.TextureCoords = coords;
            north.TextureCoords = coords;
            south.TextureCoords = coords;
            west.TextureCoords = coords;
            return this;
        }

        public BlockResourceBuilder WithTextureCoordsUp(Vector2[] coords)
        {
            if (coords.Length != 4)
            {
                throw new Exception("expected 4 texture coords");
            }

            up.TextureCoords = coords;
            return this;
        }

        public BlockResourceBuilder WithTextureCoordsDown(Vector2[] coords)
        {
            if (coords.Length != 4)
            {
                throw new Exception("expected 4 texture coords");
            }

            down.TextureCoords = coords;
            return this;
        }

        public BlockResourceBuilder WithAllColors(Vector3 col)
        {
            up.Color = col;
            down.Color = col;
            north.Color = col;
            east.Color = col;
            west.Color = col;
            south.Color = col;
            return this;
        }

        public BlockResourceBuilder WithUpColor(Vector3 col)
        {
            up.Color = col;
            return this;
        }

        public BlockResourceBuilder WithAllTex(int index)
        {
            up.TextureIndex = index;
            down.TextureIndex = index;
            north.TextureIndex = index;
            south.TextureIndex = index;
            east.TextureIndex = index;
            west.TextureIndex = index;
            return this;
        }

        public BlockResourceBuilder WithSideTex(int index)
        {
            north.TextureIndex = index;
            south.TextureIndex = index;
            east.TextureIndex = index;
            west.TextureIndex = index;
            return this;
        }

        public BlockResourceBuilder WithUpTex(int index)
        {
            up.TextureIndex = index;
            return this;
        }

        public BlockResourceBuilder WithDownTex(int index)
        {
            down.TextureIndex = index;
            return this;
        }

        public BlockResource Build()
        {
            return new BlockResource(id, up, down, north, south, east, west, meshBuilder);
        }

        private BlockFaceResource CreateDefaultFace()
        {
            return new BlockFaceResource()
            {
                Color = new Vector3(1, 1, 1),
                TextureIndex = 0,
                TextureCoords = new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                },
            };
        }
    }
}
