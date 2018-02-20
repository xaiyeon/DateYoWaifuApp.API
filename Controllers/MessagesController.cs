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
    [Route("api/users/{userId}/[controller]")]
    public class MessagesController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // Getmethod used in post
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);

        }

        // basically same method as above, but we add thread/ to make it unique url
        [HttpGet("thread/{id}")]
        public async Task<IActionResult> GetMessageThread(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessageThread(userId, id);
            var messageThread = _mapper.Map<IEnumerable<UserMessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);

        }


        //
        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, UserMessageParams usermessageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessagesForUser(usermessageParams);
            var messages = _mapper.Map<IEnumerable<UserMessageToReturnDto>>(messageFromRepo);
            Response.AddPagination(messageFromRepo.CurrentPage,
            messageFromRepo.PageSize,
            messageFromRepo.TotalCount,
            messageFromRepo.TotalPages
            );

            return Ok(messages);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody] MessageForCreationDto messageforcreationdto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageforcreationdto.SenderId = userId;
            var recipient = await _repo.GetUser(messageforcreationdto.RecipientId);
            var sender = await _repo.GetUser(messageforcreationdto.SenderId);

            // check if reciipent exists
            if (recipient == null)
            {
                return BadRequest("Master, I could not find that user!");
            }

            var message = _mapper.Map<Message>(messageforcreationdto);

            _repo.Add(message);

            // this is added because of the reversemap in mapper profiles
            var messagetoreturn = _mapper.Map<UserMessageToReturnDto>(message);

            if (await _repo.SaveAll())
            {
                // for creating the message
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messagetoreturn);
            }

            throw new Exception("Master, the service failed to message!");

        }

        // Like an HttpDelete, but we change the bool of the message not DELETE entirely
        [HttpPost("{id}")]
        public async Task<IActionResult> PostDeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId) {
                messageFromRepo.SenderDeleted = true;
            }

            if (messageFromRepo.RecipientId == userId) {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted) {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAll()) {
                return NoContent();
            }

            throw new Exception("Error deleting the message.");

        }

        // Mark as read post, this will be called multiple times and such.
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkUserMessageRead(int userId, int id) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await _repo.GetMessage(id);
            if (message.RecipientId != userId) {
                return BadRequest("Master... Uh, cannot read it!");
            }
            message.isRead = true;
            message.DateRead = DateTime.Now;
            await _repo.SaveAll();
            return NoContent();

        }


    }
}