using CategoryServices.Data;
using CategoryServices.DTOs;
using CategoryServices.Models;
using CategoryServices.Repository;
using Microsoft.EntityFrameworkCore;

namespace CategoryServices.Service
{
    public class ProductService : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly Product _product = new();

        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return _dbContext.Products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId
            }).ToList();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return null;

            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId
            };
        }
        public async Task<ProductDTO> CreateProductAsync(ProductDTO productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return productDto;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null) _dbContext.Products.Remove(product);
        }


        public async Task UpdateProductAsync(ProductDTO productDto)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == productDto.ProductId);
            if (product == null) return;

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;
            product.CategoryId = productDto.CategoryId;

            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.SaveChangesAsync();
        }

        public bool ProductAvailable(int id)
        {
            return (_dbContext.Products?.Any(b => b.ProductId == id)).GetValueOrDefault();
        }
    }
}
