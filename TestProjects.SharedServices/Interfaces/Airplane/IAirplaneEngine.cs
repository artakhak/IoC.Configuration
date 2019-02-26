namespace SharedServices.Interfaces.Airplane
{
    public interface IAirplaneEngine
    {
        IAirplaneEngineBlade Blade { get; }
        IAirplaneEngineRotor Rotor { get; }
    }
}