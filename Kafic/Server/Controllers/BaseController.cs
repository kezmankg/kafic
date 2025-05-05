using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected BaseController(ApplicationDbContext db)
        {
            _db = db;
        }

        protected string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        protected async Task<ObjectResult> InternalErrorAsync(string message, string location, string description)
        {

            Logger log = new Logger();
            log.Time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            log.Level = "Error";
            log.Message = message;
            log.Location = location;
            log.Description = description;
            _db.Loggers.Add(log);
            var changes = await _db.SaveChangesAsync();

            return StatusCode(500, message);
        }
    }
}
