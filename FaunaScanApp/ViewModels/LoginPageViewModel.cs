namespace FaunaScanApp.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly FakeAuthService _authService = new();

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [RelayCommand]
    private async Task Login()
    {
        var user = _authService.Login(Email, Password);

        // 🔍 DEBUG : afficher tous les utilisateurs
        var users = _authService.GetAllUsers();
        string debug = string.Join("\n", users.Select(u => u.Email));

        await Application.Current!.Windows[0].Page.DisplayAlert("Users enregistrés", debug, "OK");

        if (user != null)
        {
            await Application.Current!.Windows[0].Page.Navigation.PushAsync(new Views.HomePage());
        }
        else
        {
            await Application.Current!.Windows[0].Page.DisplayAlert("Erreur", "Identifiants incorrects", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Application.Current!.Windows[0].Page.Navigation.PushAsync(new Views.RegisterPage());
    }
}