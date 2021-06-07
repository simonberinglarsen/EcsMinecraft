namespace ConsoleApp1.AllSystems.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsoleApp1.AllSystems.Updating.Services;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class Frustum
    {
        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;

        private readonly float[] clipMatrix = new float[16];
        private readonly float[,] frustum = new float[6, 4];

        public enum ClippingPlane : int
        {
            Right = 0,
            Left = 1,
            Bottom = 2,
            Top = 3,
            Back = 4,
            Front = 5,
        }

        public bool PointVsFrustum(float x, float y, float z)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((frustum[i, 0] * x) + (frustum[i, 1] * y) + (frustum[i, 2] * z) + frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }

            return true;
        }

        public bool PointVsFrustum(Vector3 location)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((frustum[i, 0] * location.X) + (frustum[i, 1] * location.Y) + (frustum[i, 2] * location.Z) + frustum[i, 3] <= 0.0f)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SphereVsFrustum(float x, float y, float z, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = (frustum[p, 0] * x) + (frustum[p, 1] * y) + (frustum[p, 2] * z) + frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SphereVsFrustum(Vector3 location, float radius)
        {
            for (int p = 0; p < 6; p++)
            {
                float d = (frustum[p, 0] * location.X) + (frustum[p, 1] * location.Y) + (frustum[p, 2] * location.Z) + frustum[p, 3];
                if (d <= -radius)
                {
                    return false;
                }
            }

            return true;
        }

        public bool VolumeVsFrustum(float x, float y, float z, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((frustum[i, A] * (x - width)) + (frustum[i, B] * (y - height)) + (frustum[i, C] * (z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + width)) + (frustum[i, B] * (y - height)) + (frustum[i, C] * (z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - width)) + (frustum[i, B] * (y + height)) + (frustum[i, C] * (z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + width)) + (frustum[i, B] * (y + height)) + (frustum[i, C] * (z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - width)) + (frustum[i, B] * (y - height)) + (frustum[i, C] * (z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + width)) + (frustum[i, B] * (y - height)) + (frustum[i, C] * (z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - width)) + (frustum[i, B] * (y + height)) + (frustum[i, C] * (z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + width)) + (frustum[i, B] * (y + height)) + (frustum[i, C] * (z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public bool VolumeVsFrustum(Vector3 location, float width, float height, float length)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((frustum[i, A] * (location.X - width)) + (frustum[i, B] * (location.Y - height)) + (frustum[i, C] * (location.Z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X + width)) + (frustum[i, B] * (location.Y - height)) + (frustum[i, C] * (location.Z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X - width)) + (frustum[i, B] * (location.Y + height)) + (frustum[i, C] * (location.Z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X + width)) + (frustum[i, B] * (location.Y + height)) + (frustum[i, C] * (location.Z - length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X - width)) + (frustum[i, B] * (location.Y - height)) + (frustum[i, C] * (location.Z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X + width)) + (frustum[i, B] * (location.Y - height)) + (frustum[i, C] * (location.Z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X - width)) + (frustum[i, B] * (location.Y + height)) + (frustum[i, C] * (location.Z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (location.X + width)) + (frustum[i, B] * (location.Y + height)) + (frustum[i, C] * (location.Z + length)) + frustum[i, D] > 0)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public bool CubeVsFrustum(float x, float y, float z, float size)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((frustum[i, A] * (x - size)) + (frustum[i, B] * (y - size)) + (frustum[i, C] * (z - size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + size)) + (frustum[i, B] * (y - size)) + (frustum[i, C] * (z - size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - size)) + (frustum[i, B] * (y + size)) + (frustum[i, C] * (z - size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + size)) + (frustum[i, B] * (y + size)) + (frustum[i, C] * (z - size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - size)) + (frustum[i, B] * (y - size)) + (frustum[i, C] * (z + size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + size)) + (frustum[i, B] * (y - size)) + (frustum[i, C] * (z + size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x - size)) + (frustum[i, B] * (y + size)) + (frustum[i, C] * (z + size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                if ((frustum[i, A] * (x + size)) + (frustum[i, B] * (y + size)) + (frustum[i, C] * (z + size)) + frustum[i, D] > 0)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public void CalculateFrustum(Matrix4 projectionMatrix, Matrix4 modelViewMatrix)
        {
            clipMatrix[0] = (modelViewMatrix.M11 * projectionMatrix.M11) + (modelViewMatrix.M12 * projectionMatrix.M21) + (modelViewMatrix.M13 * projectionMatrix.M31) + (modelViewMatrix.M14 * projectionMatrix.M41);
            clipMatrix[1] = (modelViewMatrix.M11 * projectionMatrix.M12) + (modelViewMatrix.M12 * projectionMatrix.M22) + (modelViewMatrix.M13 * projectionMatrix.M32) + (modelViewMatrix.M14 * projectionMatrix.M42);
            clipMatrix[2] = (modelViewMatrix.M11 * projectionMatrix.M13) + (modelViewMatrix.M12 * projectionMatrix.M23) + (modelViewMatrix.M13 * projectionMatrix.M33) + (modelViewMatrix.M14 * projectionMatrix.M43);
            clipMatrix[3] = (modelViewMatrix.M11 * projectionMatrix.M14) + (modelViewMatrix.M12 * projectionMatrix.M24) + (modelViewMatrix.M13 * projectionMatrix.M34) + (modelViewMatrix.M14 * projectionMatrix.M44);

            clipMatrix[4] = (modelViewMatrix.M21 * projectionMatrix.M11) + (modelViewMatrix.M22 * projectionMatrix.M21) + (modelViewMatrix.M23 * projectionMatrix.M31) + (modelViewMatrix.M24 * projectionMatrix.M41);
            clipMatrix[5] = (modelViewMatrix.M21 * projectionMatrix.M12) + (modelViewMatrix.M22 * projectionMatrix.M22) + (modelViewMatrix.M23 * projectionMatrix.M32) + (modelViewMatrix.M24 * projectionMatrix.M42);
            clipMatrix[6] = (modelViewMatrix.M21 * projectionMatrix.M13) + (modelViewMatrix.M22 * projectionMatrix.M23) + (modelViewMatrix.M23 * projectionMatrix.M33) + (modelViewMatrix.M24 * projectionMatrix.M43);
            clipMatrix[7] = (modelViewMatrix.M21 * projectionMatrix.M14) + (modelViewMatrix.M22 * projectionMatrix.M24) + (modelViewMatrix.M23 * projectionMatrix.M34) + (modelViewMatrix.M24 * projectionMatrix.M44);

            clipMatrix[8] = (modelViewMatrix.M31 * projectionMatrix.M11) + (modelViewMatrix.M32 * projectionMatrix.M21) + (modelViewMatrix.M33 * projectionMatrix.M31) + (modelViewMatrix.M34 * projectionMatrix.M41);
            clipMatrix[9] = (modelViewMatrix.M31 * projectionMatrix.M12) + (modelViewMatrix.M32 * projectionMatrix.M22) + (modelViewMatrix.M33 * projectionMatrix.M32) + (modelViewMatrix.M34 * projectionMatrix.M42);
            clipMatrix[10] = (modelViewMatrix.M31 * projectionMatrix.M13) + (modelViewMatrix.M32 * projectionMatrix.M23) + (modelViewMatrix.M33 * projectionMatrix.M33) + (modelViewMatrix.M34 * projectionMatrix.M43);
            clipMatrix[11] = (modelViewMatrix.M31 * projectionMatrix.M14) + (modelViewMatrix.M32 * projectionMatrix.M24) + (modelViewMatrix.M33 * projectionMatrix.M34) + (modelViewMatrix.M34 * projectionMatrix.M44);

            clipMatrix[12] = (modelViewMatrix.M41 * projectionMatrix.M11) + (modelViewMatrix.M42 * projectionMatrix.M21) + (modelViewMatrix.M43 * projectionMatrix.M31) + (modelViewMatrix.M44 * projectionMatrix.M41);
            clipMatrix[13] = (modelViewMatrix.M41 * projectionMatrix.M12) + (modelViewMatrix.M42 * projectionMatrix.M22) + (modelViewMatrix.M43 * projectionMatrix.M32) + (modelViewMatrix.M44 * projectionMatrix.M42);
            clipMatrix[14] = (modelViewMatrix.M41 * projectionMatrix.M13) + (modelViewMatrix.M42 * projectionMatrix.M23) + (modelViewMatrix.M43 * projectionMatrix.M33) + (modelViewMatrix.M44 * projectionMatrix.M43);
            clipMatrix[15] = (modelViewMatrix.M41 * projectionMatrix.M14) + (modelViewMatrix.M42 * projectionMatrix.M24) + (modelViewMatrix.M43 * projectionMatrix.M34) + (modelViewMatrix.M44 * projectionMatrix.M44);

            frustum[(int)ClippingPlane.Right, 0] = clipMatrix[3] - clipMatrix[0];
            frustum[(int)ClippingPlane.Right, 1] = clipMatrix[7] - clipMatrix[4];
            frustum[(int)ClippingPlane.Right, 2] = clipMatrix[11] - clipMatrix[8];
            frustum[(int)ClippingPlane.Right, 3] = clipMatrix[15] - clipMatrix[12];
            NormalizePlane(frustum, (int)ClippingPlane.Right);

            frustum[(int)ClippingPlane.Left, 0] = clipMatrix[3] + clipMatrix[0];
            frustum[(int)ClippingPlane.Left, 1] = clipMatrix[7] + clipMatrix[4];
            frustum[(int)ClippingPlane.Left, 2] = clipMatrix[11] + clipMatrix[8];
            frustum[(int)ClippingPlane.Left, 3] = clipMatrix[15] + clipMatrix[12];
            NormalizePlane(frustum, (int)ClippingPlane.Left);

            frustum[(int)ClippingPlane.Bottom, 0] = clipMatrix[3] + clipMatrix[1];
            frustum[(int)ClippingPlane.Bottom, 1] = clipMatrix[7] + clipMatrix[5];
            frustum[(int)ClippingPlane.Bottom, 2] = clipMatrix[11] + clipMatrix[9];
            frustum[(int)ClippingPlane.Bottom, 3] = clipMatrix[15] + clipMatrix[13];
            NormalizePlane(frustum, (int)ClippingPlane.Bottom);

            frustum[(int)ClippingPlane.Top, 0] = clipMatrix[3] - clipMatrix[1];
            frustum[(int)ClippingPlane.Top, 1] = clipMatrix[7] - clipMatrix[5];
            frustum[(int)ClippingPlane.Top, 2] = clipMatrix[11] - clipMatrix[9];
            frustum[(int)ClippingPlane.Top, 3] = clipMatrix[15] - clipMatrix[13];
            NormalizePlane(frustum, (int)ClippingPlane.Top);

            frustum[(int)ClippingPlane.Back, 0] = clipMatrix[3] - clipMatrix[2];
            frustum[(int)ClippingPlane.Back, 1] = clipMatrix[7] - clipMatrix[6];
            frustum[(int)ClippingPlane.Back, 2] = clipMatrix[11] - clipMatrix[10];
            frustum[(int)ClippingPlane.Back, 3] = clipMatrix[15] - clipMatrix[14];
            NormalizePlane(frustum, (int)ClippingPlane.Back);

            frustum[(int)ClippingPlane.Front, 0] = clipMatrix[3] + clipMatrix[2];
            frustum[(int)ClippingPlane.Front, 1] = clipMatrix[7] + clipMatrix[6];
            frustum[(int)ClippingPlane.Front, 2] = clipMatrix[11] + clipMatrix[10];
            frustum[(int)ClippingPlane.Front, 3] = clipMatrix[15] + clipMatrix[14];
            NormalizePlane(frustum, (int)ClippingPlane.Front);
        }

        private void NormalizePlane(float[,] frustum, int side)
        {
            float magnitude = (float)Math.Sqrt((frustum[side, 0] * frustum[side, 0]) + (frustum[side, 1] * frustum[side, 1])
                                                + (frustum[side, 2] * frustum[side, 2]));
            frustum[side, 0] /= magnitude;
            frustum[side, 1] /= magnitude;
            frustum[side, 2] /= magnitude;
            frustum[side, 3] /= magnitude;
        }
    }
}
