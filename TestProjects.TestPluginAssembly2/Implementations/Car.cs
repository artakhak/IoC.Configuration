using TestPluginAssembly2.Interfaces;

namespace TestPluginAssembly2.Implementations
{
    public class Car : ICar
    {
        #region  Constructors

        public Car(IWheel wheel1)
        {
            Wheel1 = wheel1;
        }

        #endregion

        #region ICar Interface Implementation

        public IWheel Wheel1 { get; set; }
        public IWheel Wheel2 { get; set; }

        #endregion
    }
}