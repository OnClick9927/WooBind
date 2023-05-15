using System;

namespace WooBind
{
    public abstract class BindUnit : IDisposable
    {
        protected bool disposed { get; private set; }

        protected abstract void OnDispose();
        public void Dispose()
        {
            if (disposed) return;
            OnDispose();
            disposed = true;
        }
    }
}
