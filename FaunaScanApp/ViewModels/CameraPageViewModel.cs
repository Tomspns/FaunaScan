using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace FaunaScanApp.ViewModels;

public partial class CameraPageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task TakePhoto()
    {
        var photo = await MediaPicker.Default.CapturePhotoAsync();

        if (photo == null)
            return;

        await ProcessImage();
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        var photos = await MediaPicker.Default.PickPhotosAsync();
        var photo = photos.FirstOrDefault();

        if (photo == null)
            return;

        await ProcessImage();
    }

    private async Task ProcessImage()
    {
        var navigation = Application.Current!.Windows[0].Page.Navigation;

        // aller vers loading
        await navigation.PushAsync(new Views.LoadingPage());

        // simulation IA
        await Task.Delay(2000);

        // résultat fake
        var result = new Models.AnimalResult
        {
            Species = "Renard roux",
            Confidence = 0.93,
            Description = "Mammifère carnivore très adaptable.",
            Habitat = "Forêts et zones urbaines",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/1/16/2010-brown-fox.jpg"
        };

        // aller vers résultat
        await navigation.PushAsync(new Views.ResultPage(result));
    }
}