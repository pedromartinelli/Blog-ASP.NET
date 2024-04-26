using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.Utils;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AccountController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post(
            [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-"),
                PasswordHash = PasswordUtil.HashPassword(model.Password)
            };

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Name,
                    password = user.PasswordHash
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<User>("08HK2 - Não foi possível cadastrar o usuário"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<User>("08HK3 - interna no servidor"));
            }
        }

        [HttpPost("v1/accounts/login")]
        public IActionResult Login(
            [FromBody] LoginViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            try
            {
                var token = _tokenService.GenerateToken(null);

                return Ok(token);
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE13 - Falha interna no servidor"));
            }
        }
    }
}
