namespace FaunaScanApp.Services;

public class FakeAuthService
{
    private static readonly List<User> _users = new();

    public bool Register(User user)
    {
        if (_users.Any(u => u.Email == user.Email))
            return false;

        _users.Add(user);
        return true;
    }

    public User? Login(string email, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Email == email && u.Password == password);
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }
}