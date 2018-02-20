using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DateYoWaifuApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set;}
        public string Email { get; set;}
        public byte[] PasswordHash { get; set;}
        public byte[] PasswordSalt { get; set;}

        // Adding more properties
        public string ProfileImageURL { get; set;}

        public string Gender { get; set;}
        public string Ethnicity { get; set;}
        public DateTime DateOfBirth { get; set;}
        public string KnownAs { get; set;}
        public string Introduction { get; set;}
        public string LookingFor { get; set;}
        public string Interests {get; set;}
        public string City { get; set;}
        public string Country { get; set;}

        public DateTime DateCreated { get; set;}
        public DateTime LastEdit { get; set;}
        public DateTime LastActive { get; set;}

        // Don't use relationship collections in Dto 
        // This is a relationship, a one to many
        public ICollection<Photo> Photos { get; set;}

        // Relationship for User's to like each other
        public ICollection<Like> Liker { get; set;}
        public ICollection<Like> Likee { get; set;}

        // Relationship for messages
        public ICollection<Message> MessagesSent { get; set;}
        public ICollection<Message> MessagesReceived { get; set;}

        public User() {
            Photos = new Collection<Photo>();
        }


    }
}