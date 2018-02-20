using System;

namespace DateYoWaifuApp.API.Dtos
{
    public class UserMessageToReturnDto
    {
        public int Id { get; set;}
        public int SenderId { get; set;}
        public string SenderKnownAs { get; set;} // the user known as
        public string SenderPhotoUrl { get; set;}
        public int RecipientId { get; set;}
        public string RecipientKnownAs { get; set;} // the recipient known as
        public string RecipientPhotoUrl {get; set;}
        public string Title { get; set;}
        public string Content { get; set;}
        public bool isRead { get; set;}
        public DateTime? DateRead { get; set;}
        public DateTime DateSent {get; set;}
    }
}