using WinUI3Demo.Interfaces;

namespace WinUI3Demo.Extension;

public class RandomNumberGenerator : IRandomNumberGenerator
{
    private readonly Random _random = new();
    public int GetRandomNumber() => _random.Next(1, 101);
}