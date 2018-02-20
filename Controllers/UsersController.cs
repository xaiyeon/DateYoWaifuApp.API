using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DateYoWaifuApp.API.Data;
using DateYoWaifuApp.API.Dtos;
using DateYoWaifuApp.API.Helpers;
using DateYoWaifuApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DateYoWaifuApp.API.Controllers
{

    // All authorized and protected, added filter
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;
            
        }

        // We set a name for this controller
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id) {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        // Looking at parameters for pagination
        // Parameters from query
        [HttpGet]
        public async Task<IActionResult> GetUsers(UserParams userParams) {
            var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUser);
            // Copy into UserParams
            userParams.UserId = currentUser;

            // we will need to check for male, female, bisexual, nonbinary
            if (string.IsNullOrEmpty(userParams.Gender)) {
                var display_gender = "";
                // so basically think the opposite
                if (userFromRepo.Gender == "male") {
                    display_gender = "female";
                }

                if (userFromRepo.Gender == "female") {
                    display_gender = "male";
                }

                if (userFromRepo.Gender == "bisexual") {
                    display_gender = "bisexual";
                }

                // I guess this one can see other nonbinary, males, and females.
                if (userFromRepo.Gender == "nonbinary") {
                    display_gender = "nonbinary";
                }

                // Admin or Mod conditions, rare assignment
                if (userFromRepo.Gender == "admin") {
                    display_gender = "all";
                }

                // we need a query to show on match and exclusion for bisexual, and show all for nonbinary.
                userParams.Gender = display_gender;
            }


            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        // api/user/1 PUT:
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForupdatedto ) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            // Gets user ID from token
            var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(id);

            // Making sure ID exists
            if(userFromRepo == null) {
                return NotFound($"Could not find user with ID: {id}");
            }

            // Now checking user updating is same id and token
            if (currentUser != userFromRepo.Id ) {
                return Unauthorized();
            }

            // Mapping the sent data to the model using a map
            _mapper.Map(userForupdatedto, userFromRepo);

            if (await _repo.SaveAll()) {
                return NoContent(); 
                }
            
            throw new Exception($"Updating user {id} has failed, Master!");

        }

        // For liking our users
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser ( int id, int recipientId) {
            if (id !=  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }
            var like = await _repo.GetLike(id , recipientId);
            if (like != null) {
                return BadRequest("Master... You love it already!");
            }

            if (await _repo.GetUser(recipientId) == null) {
                return NotFound();
            }

            // I added a DateTime field
            like = new Like {
                LikerId = id,
                LikeeId = recipientId,
                DateCreated = DateTime.Now
                
            };

            _repo.Add<Like>(like);
            if (await _repo.SaveAll()) {
                // fixing weird like for card
                return Ok(new {});
            }

            return BadRequest("Master... I failed to like it for you!");

        }


    }
}