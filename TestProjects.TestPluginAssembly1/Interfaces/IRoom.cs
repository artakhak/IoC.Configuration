namespace TestPluginAssembly1.Interfaces
{
    public interface IRoom
    {
        #region Current Type Interface

        IDoor Door1 { get; }
        IDoor Door2 { get; }

        #endregion
    }
}