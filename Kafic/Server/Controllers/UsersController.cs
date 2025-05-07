using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Server.Data;
using Share.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsersController(ApplicationDbContext db, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IConfiguration config) : base(db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            this.roleManager = roleManager;
            _config = config;
            _mapper = mapper;
        }

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationModel userDTO)
        {
            var location = GetControllerActionNames();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                //Dodavanje kafica
                Caffe caffe = new Caffe
                {
                    Name = userDTO.CaffeName,
                    TablesNo = userDTO.TablesNo,
                    SunLoungersNo = userDTO.SunLoungersNo
                };
                _db.Caffes.Add(caffe);
                var changes = await _db.SaveChangesAsync();

                if (changes < 1)
                {
                    await transaction.RollbackAsync();
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, "Doslo je do greske prilikom kreiranja kafica");
                }

                //Dodavanje rola ako ne postoje
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    var role = new IdentityRole
                    {
                        Name = "Administrator"
                    };
                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("Waiter"))
                {
                    var role = new IdentityRole
                    {
                        Name = "Waiter"
                    };
                    await roleManager.CreateAsync(role);
                }

                //Dodavanje usera
                var username = userDTO.EmailAddress;
                var password = userDTO.Password;

                var user = new ApplicationUser
                {
                    Email = username,
                    UserName = username,
                    PhoneNumber = userDTO.PhoneNumber,
                    FullName = userDTO.FullName,
                    CaffeId = caffe.Id
                };
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var sb = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        sb.AppendLine($" {error.Description}; ");
                    }
                    await transaction.RollbackAsync();
                    return await InternalErrorAsync($"Doslo je do sledecih problema: " + sb.ToString(), location, $"{username} User Registration Attempt Failed: " + sb.ToString());
                }               

                //Dodavanje rola za usera
                await _userManager.AddToRoleAsync(user, "Waiter");
                await _userManager.AddToRoleAsync(user, "Administrator");

                // Commit transakcije
                await transaction.CommitAsync();

                return Created("login", new { result.Succeeded });
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }

        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var username = userDTO.EmailAddress;
                var password = userDTO.Password;
                var user = await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return this.Unauthorized("No user found for userName:" + username);
                }

                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    var tokenString = await GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString });
                }

                return Unauthorized(userDTO);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske prilikom logovanja, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
        }

        //[Authorize(Roles = "Administrator")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getCompany/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompanyPerEmail(string email)
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
                var caffe = await _db.Caffes.FirstOrDefaultAsync(q => q.Id == user.CaffeId);
                if (caffe == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "caffe ne postoji");
                }
                var response = _mapper.Map<CompanyModel>(caffe);

                return Ok(response);
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
            
        }

        [Route("updateCompany")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeCompany(CompanyModel companyModel)
        {
            var location = GetControllerActionNames();
            try
            {
                var caffe = await _db.Caffes.FirstOrDefaultAsync(q => q.Id == companyModel.Id);
                if (caffe == null)
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "caffe ne postoji");
                }

                caffe.Name = companyModel.Name;
                caffe.TablesNo = companyModel.TablesNo;
                caffe.SunLoungersNo = companyModel.SunLoungersNo;

                _db.Caffes.Update(caffe);

                var changes = await _db.SaveChangesAsync();

                if (changes > 0)
                {
                    return Ok();
                }
                else
                {
                    return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location,
                        "update caffe-a");
                }
            }
            catch (Exception e)
            {
                return await InternalErrorAsync("Doslo je do greske, kontaktirajte administratora", location, $"{e.Message} - {e.InnerException}");
            }
        }

        private async Task<string> GenerateJSONWebToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                , _config["Jwt:Issuer"],
                claims,
                null,
                expires: DateTime.Now.AddHours(21),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
