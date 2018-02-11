using System;
using System.ComponentModel.DataAnnotations;

namespace DateYoWaifuApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Password must be greater than 5 characters.")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto(){
            DateCreated = DateTime.Now;
            LastActive = DateTime.Now;
        }

    }
}