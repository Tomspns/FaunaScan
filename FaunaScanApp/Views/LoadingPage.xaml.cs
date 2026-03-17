using FaunaScanApp.ViewModels;

namespace FaunaScanApp.Views;

public partial class LoadingPage : ContentPage
{
    private readonly LoadingPageViewModel _viewModel;

    public LoadingPage()
    {
        InitializeComponent();

        _viewModel = new LoadingPageViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        double progress = 0;

        while (progress < 1)
        {
            progress += 0.1;
            _viewModel.Progress = progress;

            await Task.Delay(200);
        }
    }
}