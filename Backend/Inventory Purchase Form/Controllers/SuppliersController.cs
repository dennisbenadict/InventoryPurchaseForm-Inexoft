using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Purchase_Form.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierDto>>> GetAll()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create(SupplierCreateDto dto)
        {
            var supplier = await _supplierService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, supplier);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierDto>> Update(int id, SupplierCreateDto dto)
        {
            var supplier = await _supplierService.UpdateAsync(id, dto);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }
    }
}
