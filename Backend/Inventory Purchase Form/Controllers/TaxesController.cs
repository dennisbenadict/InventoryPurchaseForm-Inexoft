using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Purchase_Form.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxesController : ControllerBase
    {
        private readonly ITaxService _taxService;

        public TaxesController(ITaxService taxService)
        {
            _taxService = taxService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaxDto>>> GetAll([FromQuery] bool includeDeleted = false)
        {
            var taxes = await _taxService.GetAllAsync(includeDeleted);
            return Ok(taxes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxDto>> GetById(int id)
        {
            var tax = await _taxService.GetByIdAsync(id);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpPost]
        public async Task<ActionResult<TaxDto>> Create(TaxCreateDto dto)
        {
            var tax = await _taxService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = tax.TaxId }, tax);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaxDto>> Update(int id, TaxCreateDto dto)
        {
            var tax = await _taxService.UpdateAsync(id, dto);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var (success, message) = await _taxService.DeleteAsync(id);
            if (!success) return Conflict(new { message });
            return Ok(new { message });
        }
    }
}
