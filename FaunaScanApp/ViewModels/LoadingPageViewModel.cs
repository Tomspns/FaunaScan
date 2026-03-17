using CommunityToolkit.Mvvm.ComponentModel;

namespace FaunaScanApp.ViewModels;

public partial class LoadingPageViewModel : ObservableObject
{
    [ObservableProperty]
    private double progress;
}