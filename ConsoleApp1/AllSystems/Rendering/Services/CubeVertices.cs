namespace ConsoleApp1.AllSystems.Rendering.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public static class CubeVertices
    {
        private static readonly Dictionary<Face, int[]> VertexIndexMap = new Dictionary<Face, int[]>
        {
            { Face.Up, new int[] { 4, 3, 8, 7 } },
            { Face.Down, new int[] { 6, 5, 2, 1 } },
            { Face.North, new int[] { 5, 6, 7, 8 } },
            { Face.South, new int[] { 1, 2, 3, 4 } },
            { Face.East, new int[] { 2, 5, 8, 3 } },
            { Face.West, new int[] { 6, 1, 4, 7 } },
        };

        public static VertexBuffer FromBlockId(Vector3 localPos, ushort blockId, float shade, Face face)
        {
            var thisBlock = BlockRepository.Get(blockId);
            var aabb = thisBlock.BoundingBox;

            Vector3[] p = new Vector3[]
            {
                Vector3.Zero,
                new Vector3(aabb.MinX, aabb.MinY, aabb.MaxZ) + localPos,
                new Vector3(aabb.MaxX, aabb.MinY, aabb.MaxZ) + localPos,
                new Vector3(aabb.MaxX, aabb.MaxY, aabb.MaxZ) + localPos,
                new Vector3(aabb.MinX, aabb.MaxY, aabb.MaxZ) + localPos,
                new Vector3(aabb.MaxX, aabb.MinY, aabb.MinZ) + localPos,
                new Vector3(aabb.MinX, aabb.MinY, aabb.MinZ) + localPos,
                new Vector3(aabb.MinX, aabb.MaxY, aabb.MinZ) + localPos,
                new Vector3(aabb.MaxX, aabb.MaxY, aabb.MinZ) + localPos,
            };
            var i = VertexIndexMap[face];
            var res = BlockTextureRepository.Get(blockId);
            var faceRes = res.FaceResource(face);
            var col = faceRes.Color;
            var idx = faceRes.TextureIndex;
            var texCoords = faceRes.TextureCoords;
            return FromPointsWithLighting(idx, col * shade, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, p[i[0]], p[i[1]], p[i[2]], p[i[3]], texCoords);
        }

        public static VertexBuffer FromAabbWithCharOverlay(AxisAlignedBoundingBox aabb, Vector4 col)
        {
            var charPos = new Vector2(12, 15);
            var charOffset = Vector2.Multiply(charPos, new Vector2(12, -11) / 256f);
            var ofs = new Vector2(0, 1) + (new Vector2(2, -9) * 1 / 256f) + charOffset;
            var texQuad = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), }
                 .Select(v => (v * 8 / 256f) + ofs)
                 .ToArray();

            Vector3[] p = new Vector3[]
            {
                Vector3.Zero,
                new Vector3(aabb.MinX, aabb.MinY, aabb.MaxZ),
                new Vector3(aabb.MaxX, aabb.MinY, aabb.MaxZ),
                new Vector3(aabb.MaxX, aabb.MaxY, aabb.MaxZ),
                new Vector3(aabb.MinX, aabb.MaxY, aabb.MaxZ),
                new Vector3(aabb.MaxX, aabb.MinY, aabb.MinZ),
                new Vector3(aabb.MinX, aabb.MinY, aabb.MinZ),
                new Vector3(aabb.MinX, aabb.MaxY, aabb.MinZ),
                new Vector3(aabb.MaxX, aabb.MaxY, aabb.MinZ),
            };
            var vertices = new VertexBuffer();
            Face[] faces = new Face[] { Face.Up, Face.Down, Face.North, Face.South, Face.East, Face.West };
            foreach (var face in faces)
            {
                var i = VertexIndexMap[face];
                var p1 = p[i[0]];
                var p2 = p[i[1]];
                var p3 = p[i[2]];
                var p4 = p[i[3]];
                var v = new float[]
                {
                    p1.X, p1.Y, p1.Z, texQuad[0].X, texQuad[0].Y, col.X, col.Y, col.Z, col.W,
                    p2.X, p2.Y, p2.Z, texQuad[1].X, texQuad[1].Y, col.X, col.Y, col.Z, col.W,
                    p3.X, p3.Y, p3.Z, texQuad[2].X, texQuad[2].Y, col.X, col.Y, col.Z, col.W,
                    p1.X, p1.Y, p1.Z, texQuad[0].X, texQuad[0].Y, col.X, col.Y, col.Z, col.W,
                    p3.X, p3.Y, p3.Z, texQuad[2].X, texQuad[2].Y, col.X, col.Y, col.Z, col.W,
                    p4.X, p4.Y, p4.Z, texQuad[3].X, texQuad[3].Y, col.X, col.Y, col.Z, col.W,
                };
                vertices.AddFloatRange(v);
            }

            return vertices;
        }

        public static VertexBuffer FromPointsWithLighting(int idx, Vector3 col, float lig1, float lig2, float lig3, float lig4, float sky1, float sky2, float sky3, float sky4, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector2[] texQuad)
        {
            VertexBuffer newVertices = new VertexBuffer(256);
            newVertices.AddFloatRange(new float[] { p1.X, p1.Y, p1.Z, texQuad[0].X, texQuad[0].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig1, sky1, });

            newVertices.AddFloatRange(new float[] { p2.X, p2.Y, p2.Z, texQuad[1].X, texQuad[1].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig2, sky2, });

            newVertices.AddFloatRange(new float[] { p3.X, p3.Y, p3.Z, texQuad[2].X, texQuad[2].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig3, sky3, });

            newVertices.AddFloatRange(new float[] { p1.X, p1.Y, p1.Z, texQuad[0].X, texQuad[0].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig1, sky1, });

            newVertices.AddFloatRange(new float[] { p3.X, p3.Y, p3.Z, texQuad[2].X, texQuad[2].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig3, sky3, });

            newVertices.AddFloatRange(new float[] { p4.X, p4.Y, p4.Z, texQuad[3].X, texQuad[3].Y, idx, });
            newVertices.AddFloatRangeAsBytes(new float[] { col.X, col.Y, col.Z, lig4, sky4, });

            return newVertices;
        }
    }
}
