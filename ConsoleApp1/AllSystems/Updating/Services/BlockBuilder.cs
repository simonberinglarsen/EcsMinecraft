namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Components;

    public class BlockBuilder
    {
        private ushort? id = null;

        private bool visible = true;

        private bool isOpaque = true;

        private string name = "noname";

        private byte emittedLight = 0;

        private bool lightPasses = false;

        private bool rigid = true;

        private AxisAlignedBoundingBox aabb = null;

        private bool canHideNeighborFace = true;

        public BlockBuilder Id(ushort id)
        {
            this.id = id;
            return this;
        }

        public BlockBuilder MaxLight()
        {
            this.emittedLight = LandscapeComponent.MaxLight;
            return this;
        }

        public BlockBuilder NotRigid()
        {
            rigid = false;
            return this;
        }

        public BlockBuilder NotOpaque()
        {
            isOpaque = false;
            return this;
        }

        public BlockBuilder CannotHideNeighborFace()
        {
            canHideNeighborFace = false;
            return this;
        }

        public BlockBuilder Invisible()
        {
            visible = false;
            isOpaque = false;
            return this;
        }

        public BlockBuilder AABB(AxisAlignedBoundingBox aabb)
        {
            this.aabb = aabb;
            return this;
        }

        public BlockBuilder FullAABB()
        {
            aabb = new AxisAlignedBoundingBox
            {
                MinX = 0,
                MinY = 0,
                MinZ = 0,
                MaxX = 1,
                MaxY = 1,
                MaxZ = 1,
            };
            return this;
        }

        public BlockBuilder NoAABB()
        {
            aabb = null;
            return this;
        }

        public BlockBuilder LightPasses()
        {
            lightPasses = true;
            return this;
        }

        public Block Build()
        {
            if (!id.HasValue)
            {
                throw new Exception($"id is missing while building block {name}..");
            }

            return new Block(id.Value, aabb, emittedLight, isOpaque, name, visible, lightPasses, rigid, canHideNeighborFace);
        }
    }
}