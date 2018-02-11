using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DateYoWaifuApp.API.Data;
using DateYoWaifuApp.API.Dtos;
using DateYoWaifuApp.API.Helpers;
using DateYoWaifuApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DateYoWaifuApp.API.Controllers
{

    [Authorize]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarconfig;

        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryconfig)
        {
            _cloudinarconfig = cloudinaryconfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinarconfig.Value.CloudName,
                _cloudinarconfig.Value.ApiKey,
                _cloudinarconfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }


        // For getting photo
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            // get photo id
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);

        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoDto)
        {
            var user = await _repo.GetUser(userId);

            // We check if the userId is null or empty.
            if (String.IsNullOrEmpty(userId.ToString()))
            {
                return BadRequest("Master... I couldn't find!");

            }

            var curUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (curUserId != user.Id)
            {
                return Unauthorized();
            }

            var file = photoDto.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoDto.Url = uploadResult.Uri.ToString();
            photoDto.PublicId = uploadResult.PublicId;

            // Now setting for our DataBase
            var photo = _mapper.Map<Photo>(photoDto);
            photo.User = user;

            // check for main profile picture if not are set
            if (!user.Photos.Any(m => m.IsMain))
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                // Mapping our photo back, saving photo 
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            // if nothing
            return BadRequest("Could not add the photo...");

        }


        // Not good practice but works
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo == null)
            {
                return NotFound();
            }

            // if true
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Master, I'm sorry, this photo is your main one!");
            }

            var curMainPhoto = await _repo.GetMainPhotoForUser(userId);
            if (curMainPhoto != null)
            {
                curMainPhoto.IsMain = false;
            }
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            // something go wrong
            return BadRequest("Master! Failure of setting a photo!");


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo == null)
            {
                return NotFound();
            }

            // if true
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Master, you just can't delete your main one!");
            }

            // Cloudinary
            if (photoFromRepo.PublicId != null) {
                // Cloudinary uses public ID to delete photo
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                    {
                        _repo.Delete(photoFromRepo);

                    }
            }

            // For other photo not in Cloudinary
            if (photoFromRepo.PublicId == null) {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll()) {
                return Ok();
            }

            return BadRequest("Master... That photo was indestructible.");

        }
    }
}