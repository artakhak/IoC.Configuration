using IoC.Configuration.DiContainer;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.Collection
{
    public class Module1 : ModuleAbstr
    {
        public Module1(IEnumerable<int> values)
        {
            Values = values;
        }

        public IEnumerable<int> Values { get; }

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
