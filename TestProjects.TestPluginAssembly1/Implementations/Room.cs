using TestPluginAssembly1.Interfaces;

namespace TestPluginAssembly1.Implementations
{
    public class Room : IRoom
    {
        #region  Constructors

        public Room(IDoor door1, IDoor door2)
        {
            Door1 = door1;
            Door2 = door2;
        }

        #endregion

        #region IRoom Interface Implementation

        public IDoor Door1 { get; }
        public IDoor Door2 { get; set; }

        #endregion
    }
}