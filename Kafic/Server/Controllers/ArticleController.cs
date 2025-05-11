using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Share.Models;
using System;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public ArticleController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration config) : base(db)
        {
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }

        [Route("addGroup")]
        [HttpPost]
        public async Task<IActionResult> AddGroup([FromBody] GroupModel model)
        {
            var location = GetControllerActionNames();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.AdminEmail);
                if (user == null)
                {
                    return this.Unauthorized("No user found for userName:" + model.AdminEmail);
                }
                var group = _mapper.Map<Group>(model);
                group.CaffeId = user.CaffeId;

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
        public async Task<IActionResult> GetTeamCategoriesTenant(string email)
        {
            var location = GetControllerActionNames();
            try
            {
                var userAdmin = await _userManager.FindByEmailAsync(email);
                if (userAdmin == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "user ne postoji");
                }

                var groups = await _db.Groups.Where(q => q.CaffeId == userAdmin.CaffeId).ToListAsync();

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
                var group = await _db.Groups.FirstOrDefaultAsync(q => q.Id == int.Parse(id));

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
    }
}
