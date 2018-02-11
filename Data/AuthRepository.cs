using System;
using System.Threading.Tasks;
using DateYoWaifuApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DateYoWaifuApp.API.Data
{

    // Now we can write the actual functions

    public class AuthRepository : IAuthRepository
    {

        // For Data context, and our function for our DbContext
        private readonly DataContext _context;
        public AuthRepository(DataContext context) {
            this._context = context;
        }
        // Above is so we can use in our methods below

// Start of Login
        public async Task<User> Login(string username, string password, string email = "")
        {
            // I added email, so user can login with a username or email
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x=> x.Username == username || x.Email == email );

            if (username == null || email == null) {
                if (username == null && email == null) {
                    return null;
                } else {
                    if (username == null) {
                        return null;
                    }
                }
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null;
            }

            // auth successful
            return user;

            // throw new System.NotImplementedException();
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // So we need to pass in the key and such
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                // we need to match the array
                var computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            }

            // So we assume it passes
            return true;

            // throw new NotImplementedException();
        }

// End of Login

// Start of Register Functions
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            // to pass reference to the variable you use out, we will also have access to it in the function.
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            // Here we add our user and then save changes to Database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;

            //throw new System.NotImplementedException();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Here we take our password, turn it into a SHA encrypt and then we and SALT.
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                // Turns our password into readable bytes
                passwordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));
            }

            // throw new NotImplementedException();
        }

// End of Register Functions

        public async Task<bool> UserExists(string username, string email)
        {
            // to see if user exists or not, with username or email

            if (await _context.Users.AnyAsync(x=> x.Username == username || x.Email == email)) {
                return true;
            } else {
                return false;
            }

            // throw new System.NotImplementedException();
        }
    }
}