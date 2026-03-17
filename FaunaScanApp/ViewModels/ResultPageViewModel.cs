using CommunityToolkit.Mvvm.ComponentModel;

namespace FaunaScanApp.ViewModels;

public partial class ResultPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string species = "";

    [ObservableProperty]
    private string description = "";

    [ObservableProperty]
    private string habitat = "";
}