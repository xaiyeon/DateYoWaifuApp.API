using System.Collections.Generic;
using DateYoWaifuApp.API.Models;
using Newtonsoft.Json;

namespace DateYoWaifuApp.API.Data
{
    

    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context) {
            _context = context;
        }

        public void SeedUsers() {
            // Clear out all users and associated photos
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            // Now actually seeding
            var userData = System.IO.File.ReadAllText("./Data/Seed/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users){
                // create the password hash and salt
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("seed1234", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user.Username = user.Username.ToLower();
                _context.Users.Add(user);

            }
            // Now save
            _context.SaveChanges();

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

    }
}