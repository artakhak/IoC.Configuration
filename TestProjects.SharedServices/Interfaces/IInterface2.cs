using System;

namespace SharedServices.Interfaces
{
    public interface IInterface2
    {
        #region Current Type Interface

        DateTime Property1 { get; }
        double Property2 { get; }
        IInterface3 Property3 { get; }

        #endregion
    }
}