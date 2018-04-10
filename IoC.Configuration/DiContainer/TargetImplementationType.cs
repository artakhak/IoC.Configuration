namespace IoC.Configuration.DiContainer
{
    public enum TargetImplementationType
    {
        /// <summary>
        ///     Service is bound to its own type
        /// </summary>
        Self,

        /// <summary>
        ///     Type of implementation
        /// </summary>
        Type,

        /// <summary>
        ///     A delegate that returns implementation object
        /// </summary>
        Delegate
    }
}