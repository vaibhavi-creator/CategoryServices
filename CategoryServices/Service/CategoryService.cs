using CategoryServices.Data;
using CategoryServices.DTOs;
using CategoryServices.Models;
using CategoryServices.Repository;
using Microsoft.EntityFrameworkCore;

namespace CategoryServices.Service
{
    public class CategoryService : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly Product _product = new();

        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategorysAsync()
        {
            return _dbContext.Categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            }).ToList();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(p => p.CategoryId == id);
            if (category == null) return null;

            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return categoryDto;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null) _dbContext.Categories.Remove(category);
        }


        public async Task UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = _dbContext.Categories.FirstOrDefault(p => p.CategoryId == categoryDto.CategoryId);
            if (category == null) return;

            category.Name = categoryDto.Name;

            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChangesAsync();
        }

        public bool CategoryAvailable(int id)
        {
            return (_dbContext.Categories?.Any(c => c.CategoryId == id)).GetValueOrDefault();
        }
    }
}
