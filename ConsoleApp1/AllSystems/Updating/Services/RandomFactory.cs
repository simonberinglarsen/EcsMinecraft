namespace ConsoleApp1.AllSystems.Updating.Services
{
    using System;
    using OpenTK.Mathematics;

    public static class RandomFactory
    {
        public static Random FromColPos(Vector2i pos)
        {
            const int a = 1047484979;
            const int b = 1147485247;
            const int c = 1234567890;
            const int d = 1232343454;
            int seed = ((pos.X * a) + (pos.Y * b) + c) ^ d;
            Random rnd = new Random(seed);
            return rnd;
        }
    }
}