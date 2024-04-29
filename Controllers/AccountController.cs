using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(TokenService tokenService, PasswordHasher<User> passwordHasher)
        {
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post(
            [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var existing = await context.Users.AnyAsync(x => x.Email == model.Email);

            if (existing) return StatusCode(409, new ResultViewModel<User>("07HK5 - Email já cadastrado"));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();


                //return Ok(user.PasswordHash);
                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Name,
                    email = user.Email
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
        public async Task<IActionResult> Login(
            [FromBody] LoginViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<User>("E-mail ou senha inválidos"));

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Success)
                return StatusCode(401, new ResultViewModel<User>("E-mail ou senha inválidos"));

            try
            {
                var token = _tokenService.GenerateToken(user);

                return Ok(new ResultViewModel<dynamic>(new
                {
                    token
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE13 - Falha interna no servidor"));
            }
        }
    }
}
