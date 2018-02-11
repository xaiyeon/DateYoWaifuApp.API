using System;

namespace DateYoWaifuApp.API.Dtos
{

    // You also need to make an automapper profile for this

    public class PhotoForReturnDto
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
    }
}