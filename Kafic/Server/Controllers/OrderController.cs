using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Share.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.ApplicationUserEmail);
                if (user == null)
                {
                    return this.Unauthorized("No user found for userName:" + model.ApplicationUserEmail);
                }
                if (user.CaffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Caffe Id je null kod kreiranja porudzbine");
                }
                Order order = new Order
                {
                    DeskNo = model.DeskNo,
                    ApplicationUserEmail = model.ApplicationUserEmail,
                    Date = DateTime.Now,
                    CaffeId = (int)user.CaffeId
                };

                _db.Orders.Add(order);
                var changes = await _db.SaveChangesAsync();
                if (changes <= 0)
                {
                    await transaction.RollbackAsync();
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                    "Create order");
                }

                foreach (var article in model.ArticleModels)
                {
                    OrderArticle orderArticle = new OrderArticle
                    {
                        ArticleId = article.Id,
                        OrderId = order.Id,
                        Amount = article.Amount,
                    };
                    _db.OrderArticles.Add(orderArticle);
                }

                changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    await transaction.CommitAsync();
                    return Ok();
                }
                else
                {
                    await transaction.RollbackAsync();
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Create order");
                }
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
    }
}
