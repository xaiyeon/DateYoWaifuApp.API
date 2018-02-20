using System;

namespace DateYoWaifuApp.API.Models
{

    // Roles are for users to grant specific CRUD powers
    // TODO: Implement user roles!
    // this is a many-to-many relationship.

    public class Roles
    {
        public int Id { get; set;}
        public string Name { get; set;}
        public string ImgUrl { get; set;}
        public string Description { get; set;}
        public DateTime DateCreated { get; set;}
        public DateTime LastEdit { get; set;}

    }
}