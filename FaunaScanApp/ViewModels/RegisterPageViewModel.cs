namespace FaunaScanApp.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
    private readonly FakeAuthService _authService = new();

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private DateTime birthDate = DateTime.Now;

    [RelayCommand]
    private async Task Register()
    {
        var user = new User
        {
            Username = Username,
            Email = Email,
            Password = Password,
            BirthDate = BirthDate
        };

        var success = _authService.Register(user);

        if (success)
        {
            await Application.Current!.Windows[0].Page.DisplayAlert("Succès", "Compte créé", "OK");
            await Application.Current!.Windows[0].Page.Navigation.PopAsync();
        }
        else
        {
            await Application.Current!.Windows[0].Page.DisplayAlert("Erreur", "Email déjà utilisé", "OK");
        }
    }

    // 🔥 AJOUT IMPORTANT
    [RelayCommand]
    private async Task GoToLogin()
    {
        await Application.Current!.Windows[0].Page.Navigation.PopAsync();
    }
}