using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;
        private readonly IMapper _mapper;

        public ProductosController(DunnPharmaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/productos
        // Lista todos los productos (activos e inactivos) con su laboratorio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
        {
            var productos = await _context.Productos
                .Include(p => p.Laboratorio) // Incluimos la información del laboratorio
                .ToListAsync();

            // Mapeamos la lista de entidades Producto a una lista de ProductoDto
            var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);

            return Ok(productosDto);
        }

        // GET: api/productos/activos
        // Lista solo los productos activos con su laboratorio
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetActivos()
        {
            var productos = await _context.Productos
                .Include(p => p.Laboratorio) // 1. Incluimos el laboratorio
                .Where(p => p.Activo)
                .ToListAsync();

            // 2. Mapeamos la lista de Producto a una lista de ProductoDto
            var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);

            // 3. Devolvemos la lista de DTOs, que ya incluye el nombre del laboratorio
            return Ok(productosDto);
        }

        // POST: api/productos
        // Crear un nuevo producto y asignar precios a todos los clientes
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] CrearProductoDto dto) // <-- ✅ USA EL NUEVO DTO
        {
            // Validar si ya existe un producto con el mismo nombre
            bool existe = await _context.Productos
                .AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existe)
                return BadRequest("Ya existe un producto con ese nombre.");

            // Validar que el laboratorio exista
            bool laboratorioExiste = await _context.Laboratorios.AnyAsync(l => l.IdLaboratorio == dto.IdLaboratorio);
            if (!laboratorioExiste)
                return BadRequest("El laboratorio especificado no existe.");

            // ✅ Creamos la entidad manualmente (más simple que configurar otro AutoMapper)
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Costo = dto.Costo,
                IdLaboratorio = dto.IdLaboratorio,
                FechaRegistro = DateTime.Now,
                UsuarioRegistro = "admin", // Reemplazar por usuario autenticado
                Activo = true,
                Existencia = 0
            };

            // Guardar el producto
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // --- Tu lógica existente para generar precios por cliente (esto está perfecto y se queda igual) ---
            var clientes = await _context.Clientes
                .Include(c => c.ListaPrecio)
                .ToListAsync();

            foreach (var cliente in clientes)
            {
                var porcentaje = cliente.ListaPrecio?.PorcentajeAumento ?? 0;
                var precioCalculado = Math.Ceiling(producto.Costo + (producto.Costo * porcentaje / 100));

                var precioCliente = new PrecioCliente
                {
                    IdCliente = cliente.IdCliente,
                    IdProducto = producto.IdProducto,
                    Precio = Math.Round(precioCalculado, 2),
                    FechaRegistro = DateTime.Now,
                    UsuarioRegistro = "admin"
                };
                _context.Add(precioCliente);
            }

            await _context.SaveChangesAsync();
            // -----------------------------------------------------------------------------------------

            return Ok(new { mensaje = "Producto creado correctamente con precios asignados." });
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] ProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            // ... (tu validación de duplicados está bien) ...

            // Verificamos si el costo ha cambiado antes de actualizar
            bool costoHaCambiado = producto.Costo != dto.Costo;

            // Actualizamos los datos del producto
            producto.Nombre = dto.Nombre;
            producto.IdLaboratorio = dto.IdLaboratorio;
            producto.Costo = dto.Costo;

            // ✅ LÓGICA PARA ACTUALIZAR PRECIOS SI EL COSTO CAMBIÓ
            if (costoHaCambiado)
            {
                // Buscamos todos los precios existentes para este producto
                var preciosAActualizar = await _context.PrecioCliente
                    .Where(pc => pc.IdProducto == id)
                    .Include(pc => pc.Cliente) // Incluimos al cliente
                        .ThenInclude(c => c.ListaPrecio) // Incluimos la lista de precios del cliente
                    .ToListAsync();

                foreach (var precioCliente in preciosAActualizar)
                {
                    // Recalculamos el precio con el nuevo costo y el porcentaje de la lista del cliente
                    var porcentaje = precioCliente.Cliente.ListaPrecio?.PorcentajeAumento ?? 0;
                    precioCliente.Precio = Math.Ceiling(producto.Costo + (producto.Costo * porcentaje / 100));
                    precioCliente.FechaRegistro = DateTime.Now; // Actualizamos la fecha de modificación del precio
                    precioCliente.UsuarioRegistro = "admin"; // Reemplazar
                }
            }

            // Guardamos todos los cambios (del producto y de los precios)
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producto actualizado correctamente." });
        }

        // PATCH: api/productos/{id}/cambiar-estado
        [HttpPatch("{id}/cambiar-estado")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            producto.Activo = !producto.Activo;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado cambiado. Ahora está {(producto.Activo ? "activo" : "inactivo")}." });
        }

        // PATCH: api/Productos/{id}/reservar/{cantidad}
        [HttpPatch("{id}/reservar/{cantidad}")]
        public async Task<IActionResult> ReservarStock(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad a reservar debe ser mayor a cero.");
            }
            if (producto.Existencia < cantidad)
            {
                return BadRequest("No hay suficiente existencia para reservar la cantidad solicitada.");
            }

            producto.Existencia -= cantidad;
            await _context.SaveChangesAsync();
            return NoContent(); // Operación exitosa
        }

        // PATCH: api/Productos/{id}/liberar/{cantidad}
        [HttpPatch("{id}/liberar/{cantidad}")]
        public async Task<IActionResult> LiberarStock(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad a liberar debe ser mayor a cero.");
            }

            producto.Existencia += cantidad;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
