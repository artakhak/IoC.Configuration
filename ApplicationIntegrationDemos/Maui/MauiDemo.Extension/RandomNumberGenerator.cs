using MauiDemo.Interfaces;

namespace MauiDemo.Extension;

public class RandomNumberGenerator : IRandomNumberGenerator
{
    private readonly Random _random = new();
    public int GetRandomNumber() => _random.Next(1, 101);
}
