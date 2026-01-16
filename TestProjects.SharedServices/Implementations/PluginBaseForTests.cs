using IoC.Configuration;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public abstract class PluginBaseForTests : PluginAbstr, IPluginState
    {
        public bool IsDisposedOf { get; private set; }

        public bool IsInitialized { get; private set; }

        protected virtual void DisposeVirtual()
        {
        }

        protected virtual void InitializeVirtual()
        {
        }

        public sealed override void Dispose()
        {
            IsDisposedOf = true;
            DisposeVirtual();
        }

        public sealed override void Initialize()
        {
            IsInitialized = true;
            InitializeVirtual();
        }
    }
}