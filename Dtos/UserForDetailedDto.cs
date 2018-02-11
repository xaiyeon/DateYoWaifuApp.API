using System;
using System.Collections.Generic;
using DateYoWaifuApp.API.Models;

namespace DateYoWaifuApp.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string Username { get; set;}
        public string Email { get; set;}

        // Adding more properties
        public string ProfileImageURL { get; set;}

        public string Gender { get; set;}
        public string Ethnicity { get; set;}

        // Instead of DateOfBirth we will process and return Age
        public int Age { get; set;}
        public string KnownAs { get; set;}
        public string Introduction { get; set;}
        public string LookingFor { get; set;}
        public string Interests {get; set;}
        public string City { get; set;}
        public string Country { get; set;}

        public DateTime DateCreated { get; set;}
        public DateTime LastEdit { get; set;}
        public DateTime LastActive { get; set;}


        // Now we can bring in photos
        public ICollection<PhotoForDetailedDto> Photos { get; set;}

    }
}