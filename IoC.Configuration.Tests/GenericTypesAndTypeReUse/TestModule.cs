using System;
using System.Collections.Generic;
using System.Text;
using IoC.Configuration.DiContainer;

namespace IoC.Configuration.Tests.GenericTypesAndTypeReUse
{
    public class TestModule : ModuleAbstr
    {
        public TestModule()
        {

        }
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
