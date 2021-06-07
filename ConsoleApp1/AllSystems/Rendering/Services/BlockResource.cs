namespace ConsoleApp1.AllSystems.Rendering.Services
{
    using System.Collections.Generic;
    using ConsoleApp1.Entities;

    public class BlockResource
    {
        private Dictionary<Face, BlockFaceResource> faceToResourceMap = new Dictionary<Face, BlockFaceResource>();

        public BlockResource(
            ushort id,
            BlockFaceResource up,
            BlockFaceResource down,
            BlockFaceResource north,
            BlockFaceResource south,
            BlockFaceResource east,
            BlockFaceResource west,
            IVoxelMeshBuilder meshBuilder)
        {
            Id = id;
            faceToResourceMap[Face.Up] = up;
            faceToResourceMap[Face.Down] = down;
            faceToResourceMap[Face.North] = north;
            faceToResourceMap[Face.South] = south;
            faceToResourceMap[Face.East] = east;
            faceToResourceMap[Face.West] = west;
            Up = up;
            Down = down;
            East = east;
            West = west;
            North = north;
            South = south;
            MeshBuilder = meshBuilder;
        }

        public IVoxelMeshBuilder MeshBuilder { get; }

        public BlockFaceResource Up { get; }

        public BlockFaceResource Down { get; }

        public BlockFaceResource North { get; }

        public BlockFaceResource South { get; }

        public BlockFaceResource East { get; }

        public BlockFaceResource West { get; }

        public ushort Id { get; }

        public BlockFaceResource FaceResource(Face face)
        {
            return faceToResourceMap[face];
        }
    }
}
