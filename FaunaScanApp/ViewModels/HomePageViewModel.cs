namespace FaunaScanApp.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    private readonly INavigation _navigation;

    public HomePageViewModel(INavigation navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task Scan()
    {
        await _navigation.PushAsync(new Views.CameraPage());
    }
}