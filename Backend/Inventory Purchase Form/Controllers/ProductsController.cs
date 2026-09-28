using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Purchase_Form.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] bool? activeOnly = null)
        {
            var products = await _productService.GetAllAsync(activeOnly);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, ProductCreateDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var (success, message) = await _productService.DeleteAsync(id);
            if (!success) return Conflict(new { message });
            return Ok(new { message = "Product deleted successfully." });
        }
    }
}
