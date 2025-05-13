using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Share.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public OrderController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration config) : base(db)
        {
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [Route("addOrder")]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                //var user = await _userManager.FindByEmailAsync(model.AdminEmail);
                //if (user == null)
                //{
                //    return this.Unauthorized("No user found for userName:" + model.AdminEmail);
                //}
                //var group = _mapper.Map<Group>(model);
                //group.CaffeId = user.CaffeId;

                //_db.Groups.Add(group);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Create order");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
    }
}
