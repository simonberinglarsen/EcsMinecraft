namespace ConsoleApp1.Components
{
    using System.Collections.Generic;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class PerformanceComponent
    {
        public Entity Id { get; set; }

        public Dictionary<string, PerformanceCounter> PerformanceCounters { get; } = new Dictionary<string, PerformanceCounter>();
    }
}