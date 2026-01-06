namespace MauiDemo.RandomNumber;

public partial class RandomNumberPage : ContentPage
{
    private readonly RandomNumberViewModel _viewModel;

    public RandomNumberPage(RandomNumberViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void OnGenerateClicked(object sender, EventArgs e)
    {
        _viewModel.GenerateCommand();
    }
}