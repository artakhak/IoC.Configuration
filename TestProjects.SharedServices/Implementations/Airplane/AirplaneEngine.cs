using SharedServices.Interfaces.Airplane;

namespace SharedServices.Implementations.Airplane
{
    public class AirplaneEngine : IAirplaneEngine
    {
        public AirplaneEngine()
        {

        }
        public AirplaneEngine(IAirplaneEngineBlade blade, IAirplaneEngineRotor rotor)
        {
            Blade = blade;
            Rotor = rotor;
        }

        public IAirplaneEngineBlade Blade { get; set; }
        public IAirplaneEngineRotor Rotor { get; set; }
    }
}