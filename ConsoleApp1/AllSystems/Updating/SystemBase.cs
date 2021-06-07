namespace ConsoleApp1.AllSystems.Updating
{
    using System;
    using System.Collections.Generic;
    using ConsoleApp1.Entities;

    public abstract class SystemBase
    {
        protected void Join<TA, TB, TC, TD, TE>(
           Dictionary<Entity, TA> a,
           Dictionary<Entity, TB> b,
           Dictionary<Entity, TC> c,
           Dictionary<Entity, TD> d,
           Dictionary<Entity, TE> e,
           Action<TA, TB, TC, TD, TE> action)
        {
            foreach (var pair in a)
            {
                if (
                    b.ContainsKey(pair.Key) &&
                    c.ContainsKey(pair.Key) &&
                    d.ContainsKey(pair.Key) &&
                    e.ContainsKey(pair.Key))
                {
                    action(pair.Value, b[pair.Key], c[pair.Key], d[pair.Key], e[pair.Key]);
                }
            }
        }

        protected void Join<TA, TB, TC, TD>(
           Dictionary<Entity, TA> a,
           Dictionary<Entity, TB> b,
           Dictionary<Entity, TC> c,
           Dictionary<Entity, TD> d,
           Action<TA, TB, TC, TD> action)
        {
            foreach (var pair in a)
            {
                if (
                    b.ContainsKey(pair.Key) &&
                    c.ContainsKey(pair.Key) &&
                    d.ContainsKey(pair.Key))
                {
                    action(pair.Value, b[pair.Key], c[pair.Key], d[pair.Key]);
                }
            }
        }

        protected void Join<TA, TB, TC>(
            Dictionary<Entity, TA> a,
            Dictionary<Entity, TB> b,
            Dictionary<Entity, TC> c,
            Action<TA, TB, TC> action)
        {
            foreach (var pair in a)
            {
                if (
                    b.ContainsKey(pair.Key) &&
                    c.ContainsKey(pair.Key))
                {
                    action(pair.Value, b[pair.Key], c[pair.Key]);
                }
            }
        }

        protected void Join<TA, TB>(
           Dictionary<Entity, TA> a,
           Dictionary<Entity, TB> b,
           Action<TA, TB> action)
        {
            foreach (var pair in a)
            {
                if (b.ContainsKey(pair.Key))
                {
                    action(pair.Value, b[pair.Key]);
                }
            }
        }
    }
}