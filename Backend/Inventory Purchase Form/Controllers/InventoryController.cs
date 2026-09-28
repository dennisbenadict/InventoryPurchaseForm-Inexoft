using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Purchase_Form.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryDto>>> GetAll([FromQuery] bool excludeExpired = true)
        {
            var inventory = await _inventoryService.GetAllAsync(excludeExpired);
            return Ok(inventory);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<List<InventoryDto>>> GetByProduct(int productId, [FromQuery] bool excludeExpired = true)
        {
            var inventory = await _inventoryService.GetByProductIdAsync(productId, excludeExpired);
            return Ok(inventory);
        }
    }
}
