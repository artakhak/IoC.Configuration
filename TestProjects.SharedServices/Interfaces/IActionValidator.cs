namespace SharedServices.Interfaces
{
    public interface IActionValidator
    {
        #region Current Type Interface

        bool GetIsEnabled(int actionId);
        #endregion
    }
}