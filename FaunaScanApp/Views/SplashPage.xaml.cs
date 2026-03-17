namespace FaunaScanApp.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(100);

        Logo.Scale = 0.5;
        Logo.Opacity = 0;

        // effet zoom + fade
        await Task.WhenAll(
            Logo.FadeTo(1, 600),
            Logo.ScaleTo(1.2, 600, Easing.CubicOut)
        );

        await Logo.ScaleTo(1, 200);

        await TitleLabel.FadeTo(1, 500);
        await Subtitle.FadeTo(1, 500);

        await Task.Delay(1000);

        Application.Current!.Windows[0].Page =
            new NavigationPage(new HomePage());

        await Logo.ScaleTo(1.05, 150);
        await Logo.ScaleTo(1, 150);
    }
}