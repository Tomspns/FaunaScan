using FaunaScanApp.Services;

namespace FaunaScanApp.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly LocalDatabaseService _db = new();

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [RelayCommand]
    private async Task Login()
    {
        var user = _db.Login(Email, Password);

        if (user != null)
        {
            Application.Current!.MainPage =
                new NavigationPage(new Views.CameraPage());
        }
        else
        {
            await Application.Current!.MainPage.DisplayAlert("Erreur", "Identifiants incorrects", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Application.Current!.MainPage.Navigation.PushAsync(new Views.RegisterPage());
    }
}