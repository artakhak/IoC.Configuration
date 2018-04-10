namespace IoC.Configuration.DiContainer
{
    public enum DiResolutionScope
    {
        Transient = 1,
        Singleton = 2,
        ScopeLifetime = 3

        // TODO: Enable PerRequest, when Ninject supports ASP.NET Core.
        // In Autofac this is InstancePerRequest
        // PerRequest = 4,

        // Thread and Request scopes are not supported by Autofac. Commenting these out to avoid any non-commonly
        // implemented features.
        //Thread = 5,
    }
}