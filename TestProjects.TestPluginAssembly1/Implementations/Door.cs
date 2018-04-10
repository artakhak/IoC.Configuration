using TestPluginAssembly1.Interfaces;

namespace TestPluginAssembly1.Implementations
{
    public class Door : IDoor
    {
        #region  Constructors

        public Door(int color, double height)
        {
            Color = color;
            Height = height;
        }

        #endregion

        #region IDoor Interface Implementation

        public int Color { get; }
        public double Height { get; set; }

        #endregion
    }
}