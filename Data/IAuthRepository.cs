using System.Threading.Tasks;
using DateYoWaifuApp.API.Models;

namespace DateYoWaifuApp.API.Data
{
    // We are using the Repository Pattern to have all controllers and DB queries here for this.

    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username, string password, string email = "");
         Task<bool> UserExists(string username, string email);
    }
}