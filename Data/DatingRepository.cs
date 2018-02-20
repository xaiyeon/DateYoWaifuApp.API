using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateYoWaifuApp.API.Helpers;
using DateYoWaifuApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DateYoWaifuApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {

        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }


        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        // For getting all likes of the User
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(x => x.Likee)
                .Include(x => x.Liker)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likee.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Liker.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        // For getting the likes
        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(usr => usr.LikerId == userId && (usr.LikeeId == recipientId));
            // throw new System.NotImplementedException();
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(us => us.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            // throw new System.NotImplementedException();
        }

        // Start of User Messages
        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            // throw new System.NotImplementedException();
        }

        // uses usermessageParams
        public async Task<PagedList<Message>> GetMessagesForUser(UserMessageParams userMessageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos).AsQueryable();

            // Looking for what container it is in, checking the boolean as well
            switch (userMessageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == userMessageParams.UserId && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == userMessageParams.UserId && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == userMessageParams.UserId
                        && u.RecipientDeleted == false && u.isRead == false);
                    break;
            }

            messages = messages.OrderByDescending(d => d.DateSent);
            return await PagedList<Message>.CreateAsync(messages, userMessageParams.PageNumber, userMessageParams.PageSize);

            // throw new System.NotImplementedException();
        }

        // We want to get all messages that match the Id and vice-versa
        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => (m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId)
                    || (m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false))
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();

            return messages;
            // throw new System.NotImplementedException();
        }

        // For getting photos
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }


        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
            // throw new System.NotImplementedException();
        }

        // Gets lots of users
        // Adding paging and filter, will need to check this out and fix
        // TODO: Fix later
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(us => us.LastActive).AsQueryable();
            // lets get the user
            var ThisUser = users.Where(u => u.Id == userParams.UserId).Take(1);

            // Returning the users with page
            // Now we filter
            users = users.Where(u => u.Id != userParams.UserId); // this removes the current user


            // Then filter by gender
            // For male and female or bisexual is correct.
            if (userParams.Gender == "nonbinary" || Convert.ToBoolean(ThisUser.Where(ug => ug.Gender == "nonbinary").Any()))
            {
                // do nothing
            }
            else if (userParams.Gender == "female" || userParams.Gender == "male" || userParams.Gender == "bisexual")
            {
                users = users.Where(ug => ug.Gender == userParams.Gender);
            }
            // this is pretty useless, but left for later fixing
            else if (userParams.Gender == "nonbinary")
            {
                // we want everyone else except bisexual, I guess?
                users = users.Where(ug => ug.Gender != "bisexual");
            }
            else
            {
                // Default case
                users = users.Where(u => u.Gender == userParams.Gender);
            }

            // Now here we check for Likes
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }


            // Now for age filtering TODO: Find a better way for this
            if (userParams.MinAge != 15 || userParams.MaxAge != 9999)
            {
                // So an age between what user wants
                users = users.Where(u => u.DateOfBirth.CalculateAge() >= userParams.MinAge &&
                u.DateOfBirth.CalculateAge() <= userParams.MaxAge);
                // var min = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                // var max = DateTime.Today.AddYears(-userParams.MinAge);
                // users = users.Where(u => u.DateOfBirth >= min && u.DateOfBirth <= max);
            }

            // Checking for OrderBy, and sorting
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(us => us.DateCreated);
                        break;
                    default:
                        users = users.OrderByDescending(us => us.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            // only happens when more than save changes greater than 0
            return await _context.SaveChangesAsync() > 0;
            // throw new System.NotImplementedException();
        }
    }
}