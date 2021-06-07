namespace ConsoleApp1.Entities
{
    using System;

    [Flags]
    public enum Face
    {
        None = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        Up = 16,
        Down = 32,
        All = 63,
    }
}