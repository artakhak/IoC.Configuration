using IoC.Configuration.DiContainer;

namespace IoC.Configuration.Tests.ClassMember
{
    public class Module1 : ModuleAbstr
    {
        public Module1(int param1)
        {
            InjectedValue1 = param1;
        }

        public int InjectedValue1 { get; }

        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected override void AddServiceRegistrations()
        {
           
        }
    }
}
