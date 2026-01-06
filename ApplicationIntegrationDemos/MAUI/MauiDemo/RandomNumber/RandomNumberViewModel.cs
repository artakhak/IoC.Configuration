using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiDemo.Interfaces;

namespace MauiDemo.RandomNumber;

public class RandomNumberViewModel : INotifyPropertyChanged
{
    private readonly IRandomNumberGenerator _generator;
    private int _randomNumber;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public RandomNumberViewModel(IRandomNumberGenerator generator)
    {
        _generator = generator;
        GenerateCommand();
    }

    public int RandomNumber
    {
        get => _randomNumber;
        set
        {
            _randomNumber = value;
            OnPropertyChanged();
        }
    }

    public void GenerateCommand()
    {
        RandomNumber = _generator.GetRandomNumber();
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}