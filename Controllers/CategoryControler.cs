using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryControler : ControllerBase
    {
        [HttpGet("v1/categories")]

        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE13 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null) return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE15 - Falha interna no servidor"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = new Category(model.Name, model.Slug.ToLower());

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"/v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE9 - Não foi possível incluir a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE10 - Falha interna no servidor"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
        {

            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null) return NotFound();

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE7 - Não foi possível atualizar a categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE8 - Falha interna no servidor"));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null) return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<Category>(category));

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE5 - Não foi possível remover a categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE6 - Falha interna no servidor"));
            }
        }
    }
}
