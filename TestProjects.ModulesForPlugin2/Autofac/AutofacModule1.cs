using AutofacModule = Autofac.Module;

namespace ModulesForPlugin2.Autofac
{
    public class AutofacModule1 : AutofacModule
    {
        public AutofacModule1(int param1)
        {
            Property1 = param1;
        }
        public int Property1 { get; } 
    }
}