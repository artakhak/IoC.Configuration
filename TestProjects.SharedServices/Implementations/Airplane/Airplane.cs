using SharedServices.Interfaces.Airplane;

namespace SharedServices.Implementations.Airplane
{
    public class Airplane : IAirplane
    {
        public Airplane()
        {

        }
        public Airplane(IAirplaneEngine engine)
        {
            Engine = engine;
        }

        public IAirplaneEngine Engine { get; set; }
    }
}
