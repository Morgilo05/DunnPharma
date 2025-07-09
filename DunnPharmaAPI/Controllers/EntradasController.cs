using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EntradasController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public EntradasController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // POST: api/entradas
        // Registra una entrada de producto y su movimiento en el Cardex
        // POST: api/Entradas
        [HttpPost]
        public async Task<ActionResult> RegistrarEntrada([FromBody] EntradaProductoDto dto)
        {
            // 1) Verificar que exista el producto
            var producto = await _context.Productos.FindAsync(dto.IdProducto);
            if (producto == null)
                return NotFound("El producto especificado no existe.");

            // 2) Creamos la instancia de Lote sin hacer aún SaveChanges
            var lote = new Lote
            {
                IdProducto = dto.IdProducto,
                Piezas = dto.Piezas,
                CodigoLote = dto.CodigoLote,
                Costo = dto.Costo,
                FechaCaducidad = dto.FechaCaducidad,
                Factura = dto.Factura,
                FechaRegistro = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            // 3) Creamos el registro de Cardex y le enlazamos el lote recién creado
            var entradaCardex = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = 'E',
                Piezas = dto.Piezas,
                Lote = lote,            // ← Aquí aprovechamos la relación de navegación
                Costo = dto.Costo,
                Factura = dto.Factura,
                FechaMovimiento = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            // 4) Añadimos ambas entidades al contexto
            _context.Lotes.Add(lote);
            _context.Cardex.Add(entradaCardex);

            // 5) Actualizamos el stock (Existencia) en Productos
            producto.Existencia += dto.Piezas;
            _context.Productos.Update(producto);

            // 6) Guardamos todo en una sola operación
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Entrada registrada correctamente." });
        }


        // GET: api/entradas/historial
        // Consulta del historial del Cardex con filtros opcionales
        [HttpGet("historial")]
        public async Task<ActionResult<IEnumerable<CardexDetalleDto>>> ObtenerHistorial(
            int? idProducto = null,
            string? tipoMovimiento = null,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            var query = _context.Cardex
                .Include(c => c.Producto)
                .AsQueryable();

            // Filtro por producto
            if (idProducto.HasValue)
                query = query.Where(c => c.IdProducto == idProducto);

            // Filtro por tipo de movimiento (E, S, M, D)
            if (!string.IsNullOrWhiteSpace(tipoMovimiento))
                query = query.Where(c => c.Movimiento == tipoMovimiento[0]);

            // Filtro por fecha inicial
            if (desde.HasValue)
                query = query.Where(c => c.FechaMovimiento >= desde.Value);

            // Filtro por fecha final
            if (hasta.HasValue)
                query = query.Where(c => c.FechaMovimiento <= hasta.Value);

            // Selección final de datos proyectados al DTO
            var resultado = await query
                .OrderByDescending(c => c.FechaMovimiento)
                .Select(c => new CardexDetalleDto
                {
                    Producto = c.Producto.Nombre,
                    Movimiento = c.Movimiento == 'E' ? "Entrada" :
                                 c.Movimiento == 'S' ? "Salida" :
                                 c.Movimiento == 'M' ? "Merma" :
                                 c.Movimiento == 'D' ? "Devolución" : "Otro",
                    Piezas = c.Piezas,
                    Costo = c.Costo,
                    PrecioVenta = c.PrecioVenta,
                    Factura = c.Factura,
                    FechaMovimiento = c.FechaMovimiento,
                    Usuario = c.UsuarioRegistro
                })
                .ToListAsync();

            return Ok(resultado);

        }

        // POST: api/entradas/salida
        // Registra una salida por venta o merma
        [HttpPost("salida")]
        public async Task<ActionResult> RegistrarSalida([FromBody] SalidaProductoDto dto)
        {
            // Validar producto
            var producto = await _context.Productos.FindAsync(dto.IdProducto);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            // Validar lote
            var lote = await _context.Lotes.FindAsync(dto.IdLote);
            if (lote == null || lote.IdProducto != dto.IdProducto)
                return BadRequest("El lote especificado no corresponde al producto.");

            if (dto.Piezas > lote.Piezas)
                return BadRequest("No hay suficientes piezas disponibles en el lote.");

            // Actualizar el lote restando piezas
            lote.Piezas -= dto.Piezas;

            // Registrar en Cardex
            var salida = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = dto.Movimiento[0], // "S" o "M"
                Piezas = dto.Piezas,
                IdLote = dto.IdLote,
                Costo = dto.Costo,
                PrecioVenta = dto.PrecioVenta,
                Factura = dto.Factura,
                Nota = dto.Nota,
                FechaMovimiento = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            _context.Cardex.Add(salida);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Salida registrada correctamente." });
        }



    }
}
