using SharedServices.Interfaces.Airplane;

namespace SharedServices.Implementations.Airplane
{
    public class AirplaneEngineBlade : IAirplaneEngineBlade
    {
        public int Length { get; } = 15;
    }
}