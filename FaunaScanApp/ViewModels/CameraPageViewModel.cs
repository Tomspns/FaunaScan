namespace FaunaScanApp.ViewModels;

public partial class CameraPageViewModel : ObservableObject
{
    private readonly ApiService _apiService = new();

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            // 🔥 Permission caméra
            var status = await Permissions.RequestAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                await Application.Current!.MainPage.DisplayAlert("Erreur", "Permission caméra refusée", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo == null)
                return;

            var path = await SavePhotoAsync(photo);

            await ProcessImage(path);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            // 🔥 Permission galerie
            var status = await Permissions.RequestAsync<Permissions.Photos>();

            if (status != PermissionStatus.Granted)
            {
                await Application.Current!.MainPage.DisplayAlert("Erreur", "Permission galerie refusée", "OK");
                return;
            }

            var photos = await MediaPicker.Default.PickPhotosAsync();
            var photo = photos.FirstOrDefault();

            if (photo == null)
                return;

            var path = await SavePhotoAsync(photo);

            await ProcessImage(path);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async Task<string> SavePhotoAsync(FileResult photo)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

        using var stream = await photo.OpenReadAsync();
        using var newStream = File.Create(filePath); // 🔥 FIX Android (pas OpenWrite)

        await stream.CopyToAsync(newStream);

        return filePath;
    }

    private async Task ProcessImage(string path)
    {
        var navigation = Application.Current!.Windows[0].Page.Navigation;

        try
        {
            // 🔥 PAGE LOADING
            await navigation.PushAsync(new Views.LoadingPage());

            // 🔥 APPEL API
            var result = await _apiService.AnalyzeImageAsync(path);

            // 🔥 SÉCURITÉ
            if (result == null || result.Detections == null || result.Detections.Count == 0)
            {
                await Application.Current!.MainPage.DisplayAlert("Erreur", "Aucun animal détecté", "OK");
                await navigation.PopAsync(); // 🔥 retour loading
                return;
            }

            var detection = result.Detections[0];

            // 🔥 TRANSFORMATION API → UI
            var finalResult = new Models.AnimalResultDisplay
            {
                Species = detection.Animal,
                Confidence = detection.Confiance,
                Description = detection.Wikipedia?.DescriptionGenerale ?? "Aucune description",
                Habitat = detection.Wikipedia?.Habitat ?? "Inconnu",
                ImagePath = path
            };

            // 🔥 NAVIGATION RESULT
            await navigation.PushAsync(new Views.ResultPage(finalResult));
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage.DisplayAlert("Erreur API", ex.Message, "OK");
            await navigation.PopAsync(); // 🔥 éviter blocage sur loading
        }
    }
}