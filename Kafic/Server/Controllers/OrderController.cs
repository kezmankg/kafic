using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Share.Models;
using System.Globalization;

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
                var caffeId = await _db.Users
                    .Where(u => u.Email == model.ApplicationUserEmail)
                    .Select(u => u.CaffeId)
                    .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Caffe Id je null kod kreiranja porudzbine");
                }

                // Dohvatanje prethodnih popusta po artiklu za tog korisnika
                var previousDiscounts = await _db.Orders
                    .Where(o => o.ApplicationUserEmail == model.ApplicationUserEmail)
                    .SelectMany(o => o.OrderArticles)
                    .GroupBy(oa => oa.ArticleId)
                    .Select(g => new
                    {
                        ArticleId = g.Key,
                        Discount = g.OrderByDescending(x => x.OrderId).First().Discount // poslednji popust za svaki artikal
                    })
                    .ToDictionaryAsync(x => x.ArticleId, x => x.Discount);

                // Grupisanje artikala iz nove porudžbine i primena prethodnog popusta ako postoji
                var groupedArticles = model.ArticleModels
                    .GroupBy(a => a.Id)
                    .Select(g =>
                    {
                        var articleId = g.Key;
                        var discount = previousDiscounts.ContainsKey(articleId) ? previousDiscounts[articleId] : 0.0;

                        return new OrderArticle
                        {
                            ArticleId = articleId,
                            Amount = g.Sum(a => a.Amount),
                            Discount = discount
                        };
                    })
                    .ToList();

                var order = new Order
                {
                    DeskNo = model.DeskNo,
                    ApplicationUserEmail = model.ApplicationUserEmail,
                    Date = DateTime.UtcNow,
                    CaffeId = (int)caffeId,
                    OrderArticles = groupedArticles // direktna veza
                };

                _db.Orders.Add(order);

                var changes = await _db.SaveChangesAsync();

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
        public async Task<IActionResult> GetAllOrders(string email, string deskno)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users
                   .AsNoTracking()
                   .Where(u => u.Email == email)
                   .Select(u => u.CaffeId)
                   .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var orders = await _db.Orders
                    .AsNoTracking()
                    .Where(q => q.CaffeId == caffeId && q.DeskNo == deskno)
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
                        Price = oa.Article.Price,
                        Discount = oa.Discount
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

                var model = await _db.Orders.Include(o => o.OrderArticles)
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

        [Route("payOrder")]
        [HttpPost]
        public async Task<IActionResult> PayOrder([FromBody] PayOrderModel model)
        {
            var location = GetControllerActionNames();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var caffeId = await _db.Users
                    .Where(u => u.Email == model.UserEmail)
                    .Select(u => u.CaffeId)
                    .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }     

                var orders = await _db.Orders.Where(q => q.CaffeId == caffeId && q.DeskNo == model.DescNo)
                    .Include(c => c.OrderArticles).ThenInclude(q => q.Article)
                    .AsSplitQuery()
                    .ToListAsync();

                if (orders.Count == 0)
                {
                    return await InternalErrorAsync("Nema aktivnih porudžbina za navedeni sto.", location, "Nema narudžbina");
                }

                var orderPaids = orders.Select(order =>
                {
                    var paidArticles = order.OrderArticles.Select(oa => new OrderPaidArticle
                    {
                        ArticleId = oa.ArticleId,
                        Amount = oa.Amount,
                        Discount = oa.Discount,
                        TotalPrice = oa.Amount * oa.Article.Price * (100 - oa.Discount) / 100
                    }).ToList();

                    return new OrderPaid
                    {
                        DeskNo = order.DeskNo,
                        Date = order.Date,
                        ApplicationUserEmail = order.ApplicationUserEmail,
                        OrderArticles = paidArticles,
                        TotalPrice = paidArticles.Sum(pa => pa.TotalPrice),
                    };
                }).ToList();

                var bill = new Bill
                {
                    CaffeId = (int)caffeId,
                    Date = DateTime.UtcNow,
                    Price = model.TotalSum,
                    ApplicationUserEmail = model.UserEmail,
                    DeskNo = model.DescNo,
                    OrderPaids = orderPaids
                };

                var discount = await _db.Discounts.FirstOrDefaultAsync(q => q.CaffeId == caffeId && q.DeskNo == model.DescNo);

                if (discount != null)
                {
                    bill.Discount = discount.DiscountPercentage;
                    _db.Discounts.Remove(discount);
                }

                _db.Bills.Add(bill);

                var orders1 = await _db.Orders.Where(q => q.CaffeId == caffeId && q.DeskNo == model.DescNo).ToListAsync();
                _db.Orders.RemoveRange(orders1);

                var changes = await _db.SaveChangesAsync();

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

        [Route("updateDiscount")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDiscount(ArticleDiscountModelOrder model)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users
                   .Where(u => u.Email == model.UserEmail)
                   .Select(u => u.CaffeId)
                   .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }
                var orderArticles = await _db.OrderArticles
                    .Where(oa => oa.ArticleId == model.ArticleId &&
                                 oa.Order.CaffeId == caffeId &&
                                 oa.Order.DeskNo == model.DeskNo)
                    //.Include(oa => oa.Order)
                    .ToListAsync();

                foreach (var orderArticle in orderArticles)
                {
                    orderArticle.Discount = model.Discount;
                }

                _db.OrderArticles.UpdateRange(orderArticles);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "update group-a");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("getDiscount/{email}/{deskno}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDiscount(string email, string deskno)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users
                   .AsNoTracking()
                   .Where(u => u.Email == email)
                   .Select(u => u.CaffeId)
                   .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var discount = await _db.Discounts.AsNoTracking().FirstOrDefaultAsync(q => q.CaffeId == caffeId && q.DeskNo == deskno);

                if(discount == null)
                {
                    discount = new Discount();
                }
                var response = _mapper.Map<DiscountModel>(discount);
                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Route("updateDiscountOnBill")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBillDiscount(DiscountModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                
                var discount = _mapper.Map<Discount>(model);

                if (model.Id == 0)
                {
                    var caffeId = await _db.Users
                        .Where(u => u.Email == model.UserEmail)
                        .Select(u => u.CaffeId)
                        .FirstOrDefaultAsync();

                    if (caffeId == null) 
                    { 
                        return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                            "user ne postoji");
                    }
                    discount.CaffeId = (int)caffeId;
                }

                _db.Discounts.Update(discount);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "update discounta");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("getAllArticles/{email}/{deskno}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllArticles(string email, string deskno)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users
                    .AsNoTracking()
                    .Where(u => u.Email == email)
                    .Select(u => u.CaffeId)
                    .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Došlo je do greške, kontaktirajte administratora", location,
                        "User ne postoji");
                }

                var result = await _db.OrderArticles
                    .AsNoTracking()
                    .Where(oa => oa.Order.CaffeId == caffeId && oa.Order.DeskNo == deskno)
                    .Select(oa => new
                    {
                        oa.ArticleId,
                        oa.Article.Name,
                        oa.Article.Price,
                        oa.Discount,
                        oa.Amount
                    })
                    .ToListAsync();

                var grouped = result
                    .GroupBy(x => new { x.ArticleId, x.Name, x.Price, x.Discount })
                    .Select(g => new ArticleModelOrder
                    {
                        Name = g.Key.Name,
                        Amount = g.Sum(x => x.Amount),
                        Price = g.Key.Price,
                        Discount = g.Key.Discount
                    })
                    .ToList();

                return Ok(grouped);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpGet("getTurnover/{userEmail}/{dateFrom}/{dateTo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTurnover(string userEmail, string dateFrom, string dateTo)
        {
            var location = GetControllerActionNames();
            try
            {
                DateTime dateFromDate = DateTime.ParseExact(
                    dateFrom,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                );

                DateTime dateToDate = DateTime.ParseExact(
                    dateTo,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                );
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var totalPrice = _db.Bills
                    .Where(b => b.Date >= dateFromDate && b.Date <= dateToDate && b.Caffe != null && b.Caffe.Id == user.CaffeId)
                    .Sum(b => b.Price);

                TurnoverModel turnoverModel = new TurnoverModel
                {
                    DateFrom = dateFromDate,
                    DateTo = dateToDate,
                    TotalSum = totalPrice
                };
                return Ok(turnoverModel);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpGet("getTurnoverUser/{userEmail}/{dateFrom}/{dateTo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTurnoverUser(string userEmail, string dateFrom, string dateTo)
        {
            var location = GetControllerActionNames();
            try
            {
                DateTime dateFromDate = DateTime.ParseExact(
                    dateFrom,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                );

                DateTime dateToDate = DateTime.ParseExact(
                    dateTo,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                );

                var totalPrice = _db.OrderPaids.Include(b => b.Bill)
                    .Where(b => b.Date >= dateFromDate && b.Date <= dateToDate && b.ApplicationUserEmail == userEmail)
                    .Sum(b => b.TotalPrice);

                TurnoverModel turnoverModel = new TurnoverModel
                {
                    DateFrom = dateFromDate,
                    DateTo = dateToDate,
                    TotalSum = totalPrice,
                    UserEmail = userEmail
                };
                return Ok(turnoverModel);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
    }
}
