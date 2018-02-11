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

        public DatingRepository(DataContext context) {
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

        public Task<Photo> GetMainPhotoForUser(int userId)
        {
            return _context.Photos.Where(us => us.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            // throw new System.NotImplementedException();
        }

        // For getting photos
        public Task<Photo> GetPhoto(int id ) {
            var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }


        public async Task<User> GetUser(int id)
        {
            var  user = await _context.Users.Include(p => p.Photos ).FirstOrDefaultAsync(u => u.Id == id);
            return user;
            // throw new System.NotImplementedException();
        }

        // Gets lots of users
        // Adding paging and filter, will need to check this out and fix
        // TODO: Fix later
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var  users =  _context.Users.Include(p => p.Photos).OrderByDescending(us => us.LastActive).AsQueryable();
            // Returning the users with page
            // Now we filter
            users = users.Where(u => u.Id != userParams.UserId); // this removes the current user
            // Then filter by gender
            // For male and female or bisexual is correct.
            if (userParams.Gender == "female" || userParams.Gender == "male" || userParams.Gender == "bisexual"){
                users = users.Where(ug => ug.Gender == userParams.Gender);
            } 
            else if (userParams.Gender == "nonbinary") {
                // we want everyone else except bisexual, I guess?
                users = users.Where(ug => ug.Gender != "bisexual");
            }
            
            // Now for age filtering
            if (userParams.MinAge != 15 || userParams.MaxAge != 999999) {
                // So an age between what user wants
                users = users.Where(u => u.DateOfBirth.CalculateAge() >= userParams.MinAge &&
                u.DateOfBirth.CalculateAge() <= userParams.MaxAge
                );
            }

            // Checking for OrderBy, and sorting
            if (!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch (userParams.OrderBy) {
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