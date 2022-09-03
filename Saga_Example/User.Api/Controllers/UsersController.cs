using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.Api.Context;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _userContext;
        public UsersController(UserContext userContext)
        {
            _userContext = userContext; 
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userContext.Users.ToListAsync();
            if (users.Any())
            {
                return Ok(users);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(Entities.User request)
        {
            if (request.Age <= 0 || string.IsNullOrEmpty(request.FullName))
            {
                return BadRequest();
            }

            await _userContext.AddAsync(request);
            var res = await _userContext.SaveChangesAsync();
            return Ok(res);
        }
    }
}
