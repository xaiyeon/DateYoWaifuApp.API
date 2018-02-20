using System;

namespace DateYoWaifuApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set;}
        public int LikeeId { get; set;}
        public string Note { get; set;}
        public DateTime DateCreated { get; set;}
        public DateTime DateEdited { get; set;}

        public User Liker { get; set;}
        public User Likee { get; set;}
    }
}