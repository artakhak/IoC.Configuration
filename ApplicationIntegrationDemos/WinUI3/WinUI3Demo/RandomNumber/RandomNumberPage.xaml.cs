using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WinUI3Demo.RandomNumber;

public sealed partial class RandomNumberPage : Page
{
    public RandomNumberViewModel ViewModel { get; }

    public RandomNumberPage()
    {
        this.InitializeComponent();
        // Resolve ViewModel from DI container
        this.ViewModel = App.AppHost.Services.GetRequiredService<RandomNumberViewModel>();
    }

    private void Generate_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.GenerateCommand();
    }
}