namespace TestPluginAssembly2.Interfaces
{
    public interface ICar
    {
        #region Current Type Interface

        IWheel Wheel1 { get; set; }
        IWheel Wheel2 { get; set; }

        #endregion
    }
}