using TestPluginAssembly2.Interfaces;

namespace TestPluginAssembly2.Implementations
{
    public class Wheel : IWheel
    {
        #region  Constructors

        public Wheel(int color, double height)
        {
            Color = color;
            Height = height;
        }

        #endregion

        #region IWheel Interface Implementation

        public int Color { get; }
        public double Height { get; set; }

        #endregion
    }
}