using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DateYoWaifuApp.API.Data;
using DateYoWaifuApp.API.Dtos;
using DateYoWaifuApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DateYoWaifuApp.API.Controllers
{

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper) {

            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        // We use FromBody because that's from the JSON.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto) {

            if (string.IsNullOrEmpty(userForRegisterDto.Username)) {
                // For us, we will be storing the username always in lower-case, but user can put it any case...
                userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            }

            if (await _repo.UserExists(userForRegisterDto.Username, userForRegisterDto.Email)) {
                ModelState.AddModelError("Username", "Username already exists.");
                ModelState.AddModelError("Email", "There may already have that Email in use.");
                //return BadRequest("Username or email is already taken!");
            }
            
            // Validate data, request, based on Data Annotations
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }


            // Map to our model
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(createUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createUser.Id}, userToReturn);
            //return CreatedAtRoute();

        }


        // Login stuff
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto) {

            // An exception we will throw, uncomment it out for prod use, it's just testing
            //throw new Exception("I'm sorry UmU Master... That login didn't work. Q.Q");

            // TODO: We won't use email yet
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password, userForLoginDto.Email );

            if (userFromRepo == null) {
                return Unauthorized();
            }

            // Generating a token, json web token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Forms the payload part of our token
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey (key),
                    SecurityAlgorithms.HmacSha512Signature )

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // For profile picture
            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok( new {tokenString, user});
        }

    }
}