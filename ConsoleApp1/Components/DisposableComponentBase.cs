namespace ConsoleApp1.Components
{
    using System;
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class DisposableComponentBase : IDisposable
    {
        private bool disposed = false;

        ~DisposableComponentBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeMe()
        {
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                DisposeMe();
            }

            disposed = true;
        }
    }
}