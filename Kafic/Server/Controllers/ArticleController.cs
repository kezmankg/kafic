﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Share.Models;
using System;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ArticleController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public ArticleController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration config,
            IMemoryCache cache) : base(db)
        {
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
            _cache = cache;
        }

        [Route("addGroup")]
        [HttpPost]
        public async Task<IActionResult> AddGroup([FromBody] GroupModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users
                    .Where(u => u.Email == model.AdminEmail)
                    .Select(u => u.CaffeId)
                    .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return this.Unauthorized("No user found for userName:" + model.AdminEmail);
                }
                var group = _mapper.Map<Group>(model);
                group.CaffeId = caffeId;

                _db.Groups.Add(group);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Create group");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
           
        }

        [HttpGet("getAllGroups/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllGroups(string email)
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

                var groups = await _db.Groups.AsNoTracking().Where(q => q.CaffeId == caffeId).ToListAsync();

                var response = _mapper.Map<IList<GroupModel>>(groups);
                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getGroup/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupById(string id)
        {
            var location = GetControllerActionNames();
            try
            {
                var group = await _db.Groups.AsNoTracking().FirstOrDefaultAsync(q => q.Id == int.Parse(id));

                if (group == null)
                {
                    return NotFound();
                }

                var response = _mapper.Map<GroupModel>(group);

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Route("updateGroup")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateGroup(GroupModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var group = await _db.Groups.FirstOrDefaultAsync(q => q.Id == model.Id);
                if (group == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "grupa ne postoji");
                }

                group.Name = model.Name;
               
                _db.Groups.Update(group);

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
        [Route("addSubGroup")]
        [HttpPost]
        public async Task<IActionResult> AddSubGroup([FromBody] SubgroupModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var group = _mapper.Map<Subgroup>(model);

                _db.Subgroups.Add(group);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Create subgroup");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpGet("getGroupWithSubgroup/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllGroupByIdWithSubgroup(string email)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffeId = await _db.Users.AsNoTracking()
                    .Where(u => u.Email == email)
                    .Select(u => u.CaffeId)
                    .FirstOrDefaultAsync();

                if (caffeId == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var groups = await _db.Groups.AsNoTracking().Where(q => q.CaffeId == caffeId).Include(g => g.Subgroups).AsSplitQuery().ToListAsync();

                var response = _mapper.Map<IList<GroupModel>>(groups);
                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getSubGroup/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubGroupById(string id)
        {
            var location = GetControllerActionNames();
            try
            {
                var group = await _db.Subgroups.AsNoTracking().FirstOrDefaultAsync(q => q.Id == int.Parse(id));

                if (group == null)
                {
                    return NotFound();
                }

                var response = _mapper.Map<SubgroupModel>(group);

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
        [Route("updateSubGroup")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSubGroup(SubgroupModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var group = await _db.Subgroups.FirstOrDefaultAsync(q => q.Id == model.Id);
                if (group == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "sub grupa ne postoji");
                }

                group.GroupId = model.GroupId;
                group.Name = model.Name;

                _db.Subgroups.Update(group);

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

        [Route("addArticle")]
        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] ArticleModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var article = _mapper.Map<Article>(model);

                _db.Articles.Add(article);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    var caffeId = await _db.Users
                       .Where(u => u.Email == model.ApplicationUserEmail)
                       .Select(u => u.CaffeId)
                       .FirstOrDefaultAsync();
                    string cacheKey = $"GetAllArticles:{caffeId}";
                    _cache.Remove(cacheKey);

                    return Ok();
                }
                else
                {                   
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "Create article");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpGet("getAllArticles/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllArticles(string email)
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

                string cacheKey = $"GetAllArticles:{caffeId}";

                // Ako postoji u cache-u, vrati to
                if (_cache.TryGetValue(cacheKey, out IList<GroupModel> cachedArticles))
                {
                    return Ok(cachedArticles);
                }
                var groups = await _db.Groups.AsNoTracking()
                    .Where(g => g.CaffeId == caffeId &&
                                g.Subgroups.Any(sg => sg.Articles.Any()))
                    .Include(g => g.Subgroups.Where(sg => sg.Articles.Any()))
                        .ThenInclude(sg => sg.Articles)
                    .AsSplitQuery()
                    .ToListAsync();
                var response = _mapper.Map<IList<GroupModel>>(groups);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, response, cacheEntryOptions);

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getArticle/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticleById(string id)
        {
            var location = GetControllerActionNames();
            try
            {
                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(q => q.Id == int.Parse(id));

                if (article == null)
                {
                    return NotFound();
                }

                var response = _mapper.Map<ArticleModel>(article);

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Route("updateArticle")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateArticle(ArticleModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var article = await _db.Articles.FirstOrDefaultAsync(q => q.Id == model.Id);
                if (article == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "sub grupa ne postoji");
                }

                article.SubgroupId = model.SubgroupId;
                article.Name = model.Name;
                article.Price = model.Price;

                _db.Articles.Update(article);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    var caffeId = await _db.Users
                       .Where(u => u.Email == model.ApplicationUserEmail)
                       .Select(u => u.CaffeId)
                       .FirstOrDefaultAsync();
                    string cacheKey = $"GetAllArticles:{caffeId}";
                    _cache.Remove(cacheKey);

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

        [HttpDelete("deleteArticle/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteArticles(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var model = await _db.Articles.FirstOrDefaultAsync(q => q.Id == id);
                if(model == null)
                {
                    return NotFound();
                }
                var caffeId = await _db.Articles
                    .Where(a => a.Id == model.Id)
                    .Select(a => a.Subgroup != null && a.Subgroup.Group != null
                        ? a.Subgroup.Group.CaffeId
                        : null)
                    .FirstOrDefaultAsync();
                
                _db.Articles.Remove(model);

                var changes = await _db.SaveChangesAsync();
                if (changes > 0)
                {
                    string cacheKey = $"GetAllArticles:{caffeId}";
                    _cache.Remove(cacheKey);
                    return NoContent();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "delete artikla");
                }

            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpDelete("deleteSubgroup/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSubgroup(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var model = await _db.Subgroups.Include(g => g.Articles).AsSplitQuery().FirstOrDefaultAsync(q => q.Id == id);
                if (model == null)
                {
                    return NotFound();
                }
                _db.Subgroups.Remove(model);

                var changes = await _db.SaveChangesAsync();
                if (changes > 0)
                {
                    return NoContent();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "delete podgrupa");
                }

            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [HttpDelete("deleteGroup/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var model = await _db.Groups
                    .Include(g => g.Subgroups)
                        .ThenInclude(sg => sg.Articles)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(g => g.Id == id);
                if (model == null)
                {
                    return NotFound();
                }
                _db.Groups.Remove(model);

                var changes = await _db.SaveChangesAsync();
                if (changes > 0)
                {
                    return NoContent();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "delete podgrupa");
                }

            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }
    }
}
