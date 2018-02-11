using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DateYoWaifuApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DateYoWaifuApp.API.Helpers
{

    // For updating user's last active
    public class LogUserActivity : IAsyncActionFilter
    {
        // This is an action filter that effect our actions
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
           // throw new System.NotImplementedException();
        }
    }
}