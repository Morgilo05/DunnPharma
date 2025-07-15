using DunnPharmaAPI.Data;
using DunnPharmaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrecioClienteController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public PrecioClienteController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/PrecioCliente/5
        [HttpGet("{idCliente}")]
        public async Task<ActionResult<IEnumerable<PrecioClienteDetalleDto>>> GetPreciosPorCliente(int idCliente)
        {
            var precios = await _context.PrecioCliente
                .Where(pc => pc.IdCliente == idCliente)
                .Include(pc => pc.Producto) // Incluimos el producto para obtener su nombre y costo
                .Select(pc => new PrecioClienteDetalleDto
                {
                    IdProducto = pc.IdProducto,
                    NombreProducto = pc.Producto.Nombre,
                    CostoProducto = pc.Producto.Costo,
                    PrecioAsignado = pc.Precio
                })
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();

            if (precios == null || !precios.Any())
            {
                return NotFound("No se encontraron precios para el cliente especificado.");
            }

            return Ok(precios);
        }

        // PUT: api/PrecioCliente/5/101
        [HttpPut("{idCliente}/{idProducto}")]
        public async Task<IActionResult> UpdatePrecioCliente(int idCliente, int idProducto, [FromBody] UpdatePrecioClienteDto updateDto)
        {
            var precioCliente = await _context.PrecioCliente
                .FirstOrDefaultAsync(pc => pc.IdCliente == idCliente && pc.IdProducto == idProducto);

            if (precioCliente == null)
            {
                return NotFound("No se encontró el precio para el cliente y producto especificados.");
            }

            precioCliente.Precio = updateDto.NuevoPrecio;

            await _context.SaveChangesAsync();

            return NoContent(); // Indica que la operación fue exitosa
        }
    }
}