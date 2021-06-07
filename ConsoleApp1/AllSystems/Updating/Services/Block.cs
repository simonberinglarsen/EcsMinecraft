namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Components;

    public class Block
    {
        private AxisAlignedBoundingBox aabb;

        public Block(ushort id, AxisAlignedBoundingBox aabb, byte emittedLight, bool isOpaque, string name, bool visible, bool lightPasses, bool rigid, bool canHideNeighborFace)
        {
            Id = id;
            Rigid = rigid;
            this.aabb = aabb;
            EmittedLight = emittedLight;
            IsOpaque = isOpaque;
            Name = name;
            Visible = visible;
            LightPasses = lightPasses;
            if (aabb != null)
            {
                if (canHideNeighborFace)
                {
                    FullFaceUp = aabb.MinX == 0 && aabb.MaxX == 1 && aabb.MaxY == 1 && aabb.MinZ == 0 && aabb.MaxZ == 1;
                    FullFaceDown = aabb.MinX == 0 && aabb.MaxX == 1 && aabb.MinY == 0 && aabb.MinZ == 0 && aabb.MaxZ == 1;
                    FullFaceNorth = aabb.MinX == 0 && aabb.MaxX == 1 && aabb.MinY == 0 && aabb.MaxY == 1 && aabb.MinZ == 0;
                    FullFaceSouth = aabb.MinX == 0 && aabb.MaxX == 1 && aabb.MinY == 0 && aabb.MaxY == 1 && aabb.MaxZ == 1;
                    FullFaceEast = aabb.MaxX == 1 && aabb.MinY == 0 && aabb.MaxY == 1 && aabb.MinZ == 0 && aabb.MaxZ == 1;
                    FullFaceWest = aabb.MinX == 0 && aabb.MinY == 0 && aabb.MaxY == 1 && aabb.MinZ == 0 && aabb.MaxZ == 1;
                }

                FaceUp = aabb.MaxY == 1;
                FaceDown = aabb.MinY == 0;
                FaceNorth = aabb.MinZ == 0;
                FaceSouth = aabb.MaxZ == 1;
                FaceEast = aabb.MaxX == 1;
                FaceWest = aabb.MinX == 0;
            }

            Visible = visible;
        }

        public ushort Id { get; }

        public bool Visible { get; }

        public string Name { get; }

        public byte EmittedLight { get; }

        public bool IsOpaque { get; }

        public bool FullFaceUp { get; }

        public bool FullFaceDown { get; }

        public bool FullFaceNorth { get; }

        public bool FullFaceSouth { get; }

        public bool FullFaceEast { get; }

        public bool FullFaceWest { get; }

        public bool FaceUp { get; }

        public bool FaceDown { get; }

        public bool FaceNorth { get; }

        public bool FaceSouth { get; }

        public bool FaceEast { get; }

        public bool FaceWest { get; }

        public bool LightPasses { get; }

        public bool Rigid { get; }

        public AxisAlignedBoundingBox BoundingBox
        {
            get
            {
                return aabb?.Clone();
            }
        }
    }
}