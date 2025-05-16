using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("getAllOrders/{email}/{deskno}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamCategoriesTenant(string email, string deskno)
        {
            var location = GetControllerActionNames();
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var orders = await _db.Orders.Where(q => q.CaffeId == user.CaffeId && q.DeskNo == deskno)
                    .Include(c => c.OrderArticles)
                    .ThenInclude(k => k.Article)
                    .AsSplitQuery()
                    .ToListAsync();

                var response = orders.Select(order => new OrderModel
                {
                    Id = order.Id,
                    CaffeId = order.CaffeId,
                    DeskNo = order.DeskNo,
                    Date = order.Date,
                    ApplicationUserEmail = order.ApplicationUserEmail,
                    ArticleModels = order.OrderArticles.Select(oa => new ArticleModelOrder
                    {
                        Id = oa.ArticleId,       
                        Amount = oa.Amount,
                        Name = oa.Article.Name,
                        Price = oa.Article.Price
                    }).ToList()
                }).ToList();

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpDelete("deleteOrder/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var model = await _db.Orders
                    .FirstOrDefaultAsync(g => g.Id == id);
                if (model == null)
                {
                    return NotFound();
                }
                _db.Orders.Remove(model);

                var changes = await _db.SaveChangesAsync();
                if (changes > 0)
                {
                    return NoContent();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "delete order");
                }

            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpDelete("deleteArticle/{idOrder}/{idArticle}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteArticle(int idOrder, int idArticle)
        {
            var location = GetControllerActionNames();
            try
            {
                if (idOrder < 1 || idArticle < 1)
                {
                    return BadRequest();
                }

                var model = await _db.OrderArticles
                    .FirstOrDefaultAsync(g => g.OrderId == idOrder && g.ArticleId == idArticle);
                if (model == null)
                {
                    return NotFound();
                }
                _db.OrderArticles.Remove(model);

                var changes = await _db.SaveChangesAsync();
                if (changes > 0)
                {
                    return NoContent();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "delete order");
                }

            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
    }
}
