using SharedServices.Interfaces.Airplane;

namespace SharedServices.Implementations.Airplane
{
    public class AirplaneEngineRotor : IAirplaneEngineRotor
    {
        public AirplaneEngineRotor()
        {
            Diameter = 23;
        }

        public int Diameter { get; }
    }
}