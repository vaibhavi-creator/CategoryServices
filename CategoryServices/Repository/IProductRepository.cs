using CategoryServices.DTOs;

namespace CategoryServices.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductDTO product);
        Task UpdateProductAsync(ProductDTO product);
        Task DeleteProductAsync(int id);
        bool ProductAvailable(int id);
    }
}
