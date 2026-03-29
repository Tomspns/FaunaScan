namespace FaunaScanApp.Services;

public class LocalDatabaseService
{
    private readonly string userFile = Path.Combine(FileSystem.AppDataDirectory, "users.json");

    public List<User> GetUsers()
    {
        if (!File.Exists(userFile))
            return new List<User>();

        var json = File.ReadAllText(userFile);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public bool Register(User user)
    {
        var users = GetUsers();

        if (users.Any(u => u.Email == user.Email))
            return false;

        users.Add(user);

        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(userFile, json);

        return true;
    }

    public User? Login(string email, string password)
    {
        var users = GetUsers();

        return users.FirstOrDefault(u =>
            u.Email == email && u.Password == password);
    }
}