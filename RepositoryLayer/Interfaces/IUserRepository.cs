using ModelLayer.Models;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRepository
    {
        AppUser? GetUserByEmail(string email);
        AppUser? GetUserByGoogleId(string googleId);
        int CreateUser(AppUser user);
    }
}
