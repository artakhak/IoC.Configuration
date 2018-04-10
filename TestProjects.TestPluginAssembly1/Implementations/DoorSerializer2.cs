namespace TestPluginAssembly1.Implementations
{
    /// <summary>
    ///     This class is used to test that loading of configuration file fails when specifying two serializers
    ///     <see cref="DoorSerializer" />
    ///     and <see cref="DoorSerializer2" /> for the same type same type <see cref="Door" />.
    /// </summary>
    public class DoorSerializer2 : DoorSerializer
    {
    }
}