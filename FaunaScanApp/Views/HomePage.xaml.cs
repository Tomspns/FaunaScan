using FaunaScanApp.ViewModels;

namespace FaunaScanApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        BindingContext = new HomePageViewModel(Navigation);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Opacity = 0;
        TranslationY = 40;

        await Task.WhenAll(
            this.FadeTo(1, 600),
            this.TranslateTo(0, 0, 600, Easing.CubicOut)
        );
    }
}