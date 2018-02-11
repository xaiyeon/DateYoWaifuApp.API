using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateYoWaifuApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateYoWaifu.API.Controllers
{
    // This is an attribute route
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        // References to our db model
        private readonly DataContext _context;

        // For db and need to make available to our class
        public ValuesController(DataContext context)
        {
            _context = context;
        }

        /*
            By adding async and setting it as a task we changed our synchronous code
            to asynchronous which handles higher access.
         */

        [AllowAnonymous]
        // GET api/values
        [HttpGet]
        public async  Task<IActionResult> GetValues()
        {
            var values = await _context.Values.ToListAsync();
            return Ok(values);
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            // Need to pass ID and match ID with datbase.
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
            // return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
