using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Purchase_Form.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PurchaseDto>>> GetAll([FromQuery] string? status = null)
        {
            var purchases = await _purchaseService.GetAllAsync(status);
            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetById(int id)
        {
            var purchase = await _purchaseService.GetByIdAsync(id);
            if (purchase == null) return NotFound();
            return Ok(purchase);
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> Create(PurchaseCreateDto dto)
        {
            var purchase = await _purchaseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = purchase.PurchaseId }, purchase);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchaseDto>> Update(int id, PurchaseCreateDto dto)
        {
            try
            {
                var purchase = await _purchaseService.UpdateAsync(id, dto);
                if (purchase == null) return NotFound();
                return Ok(purchase);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var (success, message) = await _purchaseService.DeleteAsync(id);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpPost("{id}/post")]
        public async Task<ActionResult<PurchaseDto>> Post(int id)
        {
            var (success, message, purchase) = await _purchaseService.PostAsync(id);
            if (!success) return BadRequest(new { message });
            return Ok(purchase);
        }
    }
}
