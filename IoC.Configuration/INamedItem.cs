using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface INamedItem
    {
        #region Current Type Interface

        [NotNull]
        string Name { get; }

        #endregion
    }
}