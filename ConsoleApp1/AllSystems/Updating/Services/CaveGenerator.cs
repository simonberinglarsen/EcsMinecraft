namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class CaveGenerator
    {
        private int range = 8;
        private int colposx;
        private int colposz;
        private int voxelposx;
        private int voxelposz;
        private BasicColumn col;

        public CaveGenerator(BasicColumn col)
        {
            this.col = col;
            colposx = col.Position.X;
            colposz = col.Position.Y;
            voxelposx = col.Position.X * Chunk.ChunkSize;
            voxelposz = col.Position.Y * Chunk.ChunkSize;
        }

        public void Generate()
        {
            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    SpawnCave(x + colposx, z + colposz, 200);
                }
            }
        }

        private void SpawnCave(int originX, int originZ, int remainingLength, Vector3? initStart = null, double? initYaw = null, Random rnd = null)
        {
            const int maxDist = 7 * 7 * Chunk.ChunkSize * Chunk.ChunkSize;

            if (remainingLength < 10)
            {
                return;
            }

            if (rnd == null)
            {
                rnd = RandomFactory.FromColPos(new Vector2i(originX, originZ));
                var triggerCave = rnd.NextDouble() < 0.01;
                if (!triggerCave)
                {
                    return;
                }
            }

            Vector3 pStart;
            if (initStart == null)
            {
                pStart = new Vector3((originX * Chunk.ChunkSize) + 8, 40, (originZ * Chunk.ChunkSize) + 8);
            }
            else
            {
                pStart = initStart.Value;
            }

            Vector3 p = new Vector3(pStart);
            double yaw;
            if (initYaw == null)
            {
                yaw = rnd.NextDouble() * Math.PI * 2.0;
            }
            else
            {
                yaw = initYaw.Value;
            }

            var ws = new LandscapeService();
            var dir = Vector3.Zero;
            while (remainingLength > 0)
            {
                dir.X = (float)Math.Cos(yaw);
                dir.Y = ((float)rnd.NextDouble() - 0.5f) * 2f;
                dir.Z = (float)Math.Sin(yaw);
                dir.Normalize();
                var length = rnd.Next(8, 16);
                var tunnelSize = rnd.Next(2, 6);
                if (rnd.NextDouble() < 0.3)
                {
                    var newYaw = yaw + ((Math.PI / 4f) * (rnd.NextDouble() * 3f));
                    SpawnCave(originX, originZ, (2 * remainingLength) / 3, p, newYaw, rnd);
                }

                for (int l = 0; l < length; l++)
                {
                    if ((pStart - p).LengthSquared >= maxDist)
                    {
                        return;
                    }

                    var pi = ws.ToVector3i(p);
                    if (pi.X >= voxelposx - tunnelSize && pi.X < voxelposx + 16 + tunnelSize
                        && pi.Z >= voxelposz - tunnelSize && pi.Z < voxelposz + 16 + tunnelSize)
                    {
                        CarveHalfCircle(
                            pi.X - voxelposx,
                            pi.Y,
                            pi.Z - voxelposz,
                            tunnelSize);
                    }

                    p += 1.5f * dir;
                }

                yaw += rnd.NextDouble() * 0.1;
                if (yaw < 0)
                {
                    yaw += Math.PI * 2.0f;
                }
                else if (yaw >= Math.PI * 2.0f)
                {
                    yaw -= Math.PI * 2.0f;
                }

                remainingLength -= length;
            }
        }

        private void CarveHalfCircle(int x, int y, int z, int r)
        {
            for (int x1 = -r; x1 <= r; x1++)
            {
                for (int y1 = 0; y1 <= r; y1++)
                {
                    for (int z1 = -r; z1 <= r; z1++)
                    {
                        if ((x1 * x1) + (y1 * y1) + (z1 * z1) >= (r * r))
                        {
                            continue;
                        }

                        int x2 = x + x1;
                        int y2 = y + y1;
                        int z2 = z + z1;
                        if (x2 < 0 || x2 >= 16
                            || y2 < 0 || y2 >= 256
                            || z2 < 0 || z2 >= 16)
                        {
                            continue;
                        }

                        col.SetVoxel(x2, y2, z2, BlockId.Void);
                    }
                }
            }
        }
    }
}