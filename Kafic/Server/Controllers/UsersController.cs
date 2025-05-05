using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Share.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationModel userDTO)
        {
            Logger log = new Logger();
            log.Time = "testTime";
            log.Level = "testLevel";
            log.Message = "testMessage";
            log.Location = "testLocation";
            log.Description = "testDescription";
            _db.Loggers.Add(log);
            var changes = await _db.SaveChangesAsync();
            if(changes < 1)
            {
                //return InternalError
            }
            return Ok();
        }

        [HttpGet("register/test")]
        public async Task<IActionResult> GetUserPerEmail()
        {
            return Ok("test");
        }
    }
}
