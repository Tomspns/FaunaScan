using FaunaScanApp.Services;
using FaunaScanApp.Models;

namespace FaunaScanApp.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
    private readonly LocalDatabaseService _db = new();

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

        var success = _db.Register(user);

        if (success)
        {
            await Application.Current!.MainPage.DisplayAlert("Succès", "Compte créé", "OK");
            await Application.Current!.MainPage.Navigation.PopAsync();
        }
        else
        {
            await Application.Current!.MainPage.DisplayAlert("Erreur", "Email déjà utilisé", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Application.Current!.MainPage.Navigation.PopAsync();
    }
}