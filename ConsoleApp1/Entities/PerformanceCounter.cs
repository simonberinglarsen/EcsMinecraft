namespace ConsoleApp1.Entities
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class PerformanceCounter
    {
        public PerformanceCounter(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public long Count { get; set; } = 0;

        public Stopwatch Stopwatch { get; } = new Stopwatch();
    }
}