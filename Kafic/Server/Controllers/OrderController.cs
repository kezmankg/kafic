using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;

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
    }
}
