using System;

namespace DateYoWaifuApp.API.Models
{
    public class Photo
    {
        public int Id { get; set;}
        public string Url { get; set;}
        public string Title { get; set;}
        public string Description { get; set;}
        public DateTime DateUploaded { get; set;}
        public DateTime LastEdited { get; set;}
        public bool IsMain { get; set;}
        public bool IsCover { get; set;}

        public string PublicId { get; set;}

        public User User { get; set;}
        public int UserId { get; set;}

    }
}