using System.Linq;
using AutoMapper;
using DateYoWaifuApp.API.Dtos;
using DateYoWaifuApp.API.Models;

namespace DateYoWaifuApp.API.Helpers
{

    // This is used for telling AutoMapper how to map our DTO and data/model together

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            // You can set the properties, here we are getting the photo with IsMain and passing the photo
            // URL to our UserModel to display
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.ProfileImageURL, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                })
            ;

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.ProfileImageURL, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                })
            ;

            CreateMap<Photo, PhotoForDetailedDto>();
            // For updating user
            CreateMap<UserForUpdateDto, User>();

            // For photo handlings
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();

            // For register users
            CreateMap<UserForRegisterDto, User>();

            // Messages, we use reversemap
            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            // Map for messages of return, we Map the main photo url to the Dto
            CreateMap<Message, UserMessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(
                    p => p.IsMain).Url
                ))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(
                    p => p.IsMain).Url
                ));
                
                

        }

    }
}
