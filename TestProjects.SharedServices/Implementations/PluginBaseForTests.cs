using IoC.Configuration;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public abstract class PluginBaseForTests : PluginAbstr, IPluginState
    {
        #region IPluginState Interface Implementation

        public bool IsDisposedOf { get; private set; }

        public bool IsInitialized { get; private set; }

        #endregion

        #region Current Type Interface

        protected virtual void DisposeVirtual()
        {
        }

        protected virtual void InitializeVirtual()
        {
        }

        #endregion

        #region Member Functions

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

        #endregion
    }
}