using Blog.Data;
using Blog.Models;
using Blog.ViewModel;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostController : ControllerBase
    {
        private readonly BlogDataContext _context;

        public PostController(BlogDataContext context)
        {
            _context = context;
        }

        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await _context.Posts.CountAsync();
                var items = await _context
                    .Posts
                    .AsNoTracking()
                    .Select(x => new ListPostsVM
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})",
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts = items
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("02X05 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync
            ([FromRoute] int id)
        {
            try
            {
                var item = await _context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return NotFound(new ResultViewModel<Post>("Item não encontrado"));

                return Ok(new ResultViewModel<Post>(item));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("02X05 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromRoute] string category,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await _context.Posts.Where(x => x.Category.Name == category).CountAsync();

                var items = await _context
                    .Posts
                    .AsNoTracking()
                    .Where(x => x.Category.Slug == category)
                    .Select(x => new ListPostsVM
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})",
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts = items
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("02X05 - Falha interna no servidor"));
            }
        }
    }
}
