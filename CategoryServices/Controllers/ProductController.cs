using CategoryServices.DTOs;
using CategoryServices.Models;
using CategoryServices.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CategoryServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController( IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var product = await _productRepository.GetAllProductsAsync();
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdProduct = await _productRepository.CreateProductAsync(productDto);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut]
        public async Task<ActionResult<ProductDTO>> PutProduct(int id, ProductDTO productDto)
        {
            if (id != productDto.ProductId) return BadRequest();
            try
            {
                await _productRepository.UpdateProductAsync(productDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                _productRepository.ProductAvailable(id);
                if (!_productRepository.ProductAvailable(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
