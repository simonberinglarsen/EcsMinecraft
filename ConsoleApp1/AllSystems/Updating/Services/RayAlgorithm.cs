namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class RayAlgorithm
    {
        public RayAlgorithm()
        {
        }

        public void March(Func<int, int, int, Face, bool> callBack, Vector3 p0, Vector3 end)
        {
            var d = end - p0;
            float totalLength = d.Length;
            d.Normalize();

            int sx = Math.Sign(d.X);
            int sy = Math.Sign(d.Y);
            int sz = Math.Sign(d.Z);
            int vx = (int)Math.Floor(p0.X);
            int vy = (int)Math.Floor(p0.Y);
            int vz = (int)Math.Floor(p0.Z);
            Face face = Face.None;
            while (true)
            {
                if (callBack != null)
                {
                    bool cancel = callBack(vx, vy, vz, face);
                    if (cancel)
                    {
                        break;
                    }
                }

                float x2 = vx + (sx < 0 ? 0 : 1);
                float y2 = vy + (sy < 0 ? 0 : 1);
                float z2 = vz + (sz < 0 ? 0 : 1);
                float tx = (x2 - p0.X) / d.X;
                float ty = (y2 - p0.Y) / d.Y;
                float tz = (z2 - p0.Z) / d.Z;

                if (tx < ty && tx < tz)
                {
                    vx += sx;
                    totalLength -= tx;
                    p0.X = x2;
                    p0.Y = (tx * d.Y) + p0.Y;
                    p0.Z = (tx * d.Z) + p0.Z;
                    face = sx < 0 ? Face.East : Face.West;
                }
                else if (ty < tx && ty < tz)
                {
                    vy += sy;
                    totalLength -= ty;
                    p0.X = (ty * d.X) + p0.X;
                    p0.Y = y2;
                    p0.Z = (ty * d.Z) + p0.Z;
                    face = sy < 0 ? Face.Up : Face.Down;
                }
                else
                {
                    vz += sz;
                    totalLength -= tz;
                    p0.X = (tz * d.X) + p0.X;
                    p0.Y = (tz * d.Y) + p0.Y;
                    p0.Z = z2;
                    face = sz < 0 ? Face.South : Face.North;
                }

                if (totalLength < 0.001f)
                {
                    break;
                }
            }
        }
    }
}