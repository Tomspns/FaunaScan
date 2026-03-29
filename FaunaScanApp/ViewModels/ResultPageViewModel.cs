namespace FaunaScanApp.ViewModels;

public partial class ResultPageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task ScanAgain()
    {
        // 🔥 RESET COMPLET → marche à 100%
        Application.Current!.MainPage =
            new NavigationPage(new Views.CameraPage());
    }
}