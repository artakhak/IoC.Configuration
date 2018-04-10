namespace SharedServices.Interfaces
{
    /// <summary>
    ///     IInterface3 should have a single implementation binding not to break Ninject tests.
    /// </summary>
    public interface IInterface3
    {
        #region Current Type Interface

        int Property1 { get; }
        IInterface4 Property2 { get; }

        #endregion
    }
}