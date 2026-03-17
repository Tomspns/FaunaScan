namespace FaunaScanApp.Views;

public partial class CameraPage : ContentPage
{
    public CameraPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.CameraPageViewModel();
    }
}