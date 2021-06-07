namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.AllSystems.Rendering;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class AxisAlignedBoundingBox
    {
        public float MinX { get; set; }

        public float MinY { get; set; }

        public float MinZ { get; set; }

        public float MaxX { get; set; }

        public float MaxY { get; set; }

        public float MaxZ { get; set; }

        public Vector3 Min
        {
            get
            {
                return new Vector3(MinX, MinY, MinZ);
            }
        }

        public Vector3 Max
        {
            get
            {
                return new Vector3(MaxX, MaxY, MaxZ);
            }
        }

        public AxisAlignedBoundingBox MoveTo(Vector3 vector3)
        {
            MaxX = vector3.X + MaxX;
            MaxY = vector3.Y + MaxY;
            MaxZ = vector3.Z + MaxZ;
            MinX = vector3.X + MinX;
            MinY = vector3.Y + MinY;
            MinZ = vector3.Z + MinZ;
            return this;
        }

        public AxisAlignedBoundingBox Clone()
        {
            return new AxisAlignedBoundingBox
            {
                MinX = this.MinX,
                MinY = this.MinY,
                MinZ = this.MinZ,
                MaxX = this.MaxX,
                MaxY = this.MaxY,
                MaxZ = this.MaxZ,
            };
        }

        public bool PointInside(Vector3 p)
        {
            return
                p.X >= MinX &&
                p.Y >= MinY &&
                p.Z >= MinZ &&
                p.X <= MaxX &&
                p.Y <= MaxY &&
                p.Z <= MaxZ;
        }

        public bool Intersect(AxisAlignedBoundingBox them)
        {
            return (them.MinX < MaxX && them.MaxX > MinX) &&
                    (them.MinY < MaxY && them.MaxY > MinY) &&
                    (them.MinZ < MaxZ && them.MaxZ > MinZ);
        }

        public void Expand(AxisAlignedBoundingBox him)
        {
            MinX = Math.Min(MinX, him.MinX);
            MinY = Math.Min(MinY, him.MinY);
            MinZ = Math.Min(MinZ, him.MinZ);
            MaxX = Math.Max(MaxX, him.MaxX);
            MaxY = Math.Max(MaxY, him.MaxY);
            MaxZ = Math.Max(MaxZ, him.MaxZ);
        }
    }
}