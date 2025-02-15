using CategoryServices.DTOs;
using CategoryServices.Models;
using CategoryServices.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CategoryServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllCategorysAsync();
            if (categories == null) { return NotFound(); }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> PostCategories(CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdCategory = await _categoryRepository.CreateCategoryAsync(categoryDto);

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
        }

        [HttpPut]
        public async Task<ActionResult<CategoryDTO>> PutCategories(int id, CategoryDTO categoryDto)
        {
            if (id != categoryDto.CategoryId) return BadRequest();
            try
            {
                await _categoryRepository.UpdateCategoryAsync(categoryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                _categoryRepository.CategoryAvailable(id);
                if (!_categoryRepository.CategoryAvailable(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
