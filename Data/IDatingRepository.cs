using System.Collections.Generic;
using System.Threading.Tasks;
using DateYoWaifuApp.API.Helpers;
using DateYoWaifuApp.API.Models;

namespace DateYoWaifuApp.API.Data
{
    public interface IDatingRepository
    {
        // This is a generics which take different types
         void Add<T>(T entity) where T: class;

        void Delete<T>(T entity) where T: class;

        Task<bool> SaveAll();

        // This was changed
        // Task<IEnumerable<User>> GetUsers();

        Task<PagedList<User>> GetUsers(UserParams userParams);

        Task<User> GetUser(int id);

        // New tasks
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);

        Task<Like> GetLike(int userId, int recipientId);

        // message id
        Task<Message> GetMessage(int id);

        // Hm
        Task<PagedList<Message>> GetMessagesForUser(UserMessageParams usermessageParams);

        // Get messages sent by two users
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);

    }
}