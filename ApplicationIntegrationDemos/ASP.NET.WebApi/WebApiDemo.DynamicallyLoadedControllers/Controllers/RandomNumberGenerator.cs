namespace WebApiDemo.DynamicallyLoadedControllers.Controllers;

public class RandomNumberGenerator : IRandomNumberGenerator
{
    private readonly Random _random = new Random(100);

    public int Generate()
    {
        return _random.Next(1000);
    }
}