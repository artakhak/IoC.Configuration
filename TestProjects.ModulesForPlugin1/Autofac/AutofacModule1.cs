using Autofac;
using SharedServices.Interfaces;
using TestPluginAssembly1.Implementations;
using AutofacModule = Autofac.Module;

namespace ModulesForPlugin1.Autofac
{
    public class AutofacModule1 : AutofacModule
    {
        #region  Constructors

        public AutofacModule1(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Interface1_Impl1>().As<IInterface1>().SingleInstance();
        }

        public int Property1 { get; }

        #endregion
    }
}