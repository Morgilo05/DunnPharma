using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.DTOs;
using DunnPharmaAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;
        private readonly IMapper _mapper;

        public ClientesController(DunnPharmaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
        }

        // GET: api/clientes/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetActivos()
        {
            var clientes = await _context.Clientes
                .Where(c => c.Activo)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] ClienteDto dto)
        {
            // 1. Validaciones iniciales
            if (await _context.Clientes.AnyAsync(c => c.Nombre.ToLower() == dto.Nombre.ToLower()))
                return BadRequest("Ya existe un cliente con ese nombre.");

            var listaDePrecios = await _context.ListasPrecio.FindAsync(dto.IdLista);
            if (listaDePrecios == null)
                return BadRequest("La lista de precios asignada no existe.");

            // 2. Crear y guardar el nuevo cliente
            var cliente = _mapper.Map<Cliente>(dto);
            cliente.FechaRegistro = DateTime.Now;
            cliente.UsuarioRegistro = "admin"; // Reemplazar por usuario autenticado
            cliente.Activo = true;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(); // Guardamos para obtener el IdCliente nuevo

            // 3. ✅ LÓGICA PARA GENERAR PRECIOS AUTOMÁTICAMENTE
            // Obtenemos todos los productos que están activos
            var productosActivos = await _context.Productos.Where(p => p.Activo).ToListAsync();

            foreach (var producto in productosActivos)
            {
                // Calculamos el precio según el porcentaje de la lista asignada al cliente
                decimal precioCalculado = Math.Ceiling(producto.Costo + (producto.Costo * listaDePrecios.PorcentajeAumento / 100));

                var precioCliente = new PrecioCliente
                {
                    IdCliente = cliente.IdCliente, // Usamos el ID del cliente recién creado
                    IdProducto = producto.IdProducto,
                    Precio = Math.Round(precioCalculado, 2),
                    FechaRegistro = DateTime.Now,
                    UsuarioRegistro = "admin" // Reemplazar
                };
                _context.PrecioCliente.Add(precioCliente);
            }

            // 4. Guardamos todos los nuevos precios generados
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente creado correctamente con precios asignados." });
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] EditarClienteDto editarDto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            // Validación de duplicados al editar
            if (await _context.Clientes.AnyAsync(c => c.Nombre.ToLower() == editarDto.Nombre.ToLower() && c.IdCliente != id))
            {
                return BadRequest("Ya existe otro cliente con ese nombre.");
            }

            // Verificamos si la lista de precios ha cambiado antes de hacer cualquier cosa
            bool listaHaCambiado = cliente.IdLista != editarDto.IdLista;

            // Actualizamos los datos del cliente desde el DTO
            cliente.Nombre = editarDto.Nombre;
            cliente.IdLista = editarDto.IdLista;

            // ✅ NUEVA LÓGICA: Si la lista de precios cambió, recalculamos todo
            if (listaHaCambiado)
            {
                // Buscamos la nueva lista de precios para obtener su porcentaje
                var nuevaLista = await _context.ListasPrecio.FindAsync(editarDto.IdLista);
                if (nuevaLista == null)
                {
                    return BadRequest("La nueva lista de precios asignada no existe.");
                }

                // Buscamos todos los precios existentes para este cliente, incluyendo los datos del producto
                var preciosAActualizar = await _context.PrecioCliente
                    .Where(pc => pc.IdCliente == id)
                    .Include(pc => pc.Producto) // Incluimos el producto para tener acceso a su costo
                    .ToListAsync();

                foreach (var precioCliente in preciosAActualizar)
                {
                    // Recalculamos el precio con el costo del producto y el NUEVO porcentaje
                    decimal precioCalculado = Math.Ceiling(precioCliente.Producto.Costo + (precioCliente.Producto.Costo * nuevaLista.PorcentajeAumento / 100));
                    precioCliente.Precio = precioCalculado;
                }
            }

            // Guardamos todos los cambios (del cliente y de los precios) en una sola transacción
            await _context.SaveChangesAsync();

            return NoContent(); // Indica que la operación fue exitosa pero no devuelve contenido
        }

        // PATCH: api/clientes/{id}/cambiar-estado
        [HttpPatch("{id}/cambiar-estado")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            cliente.Activo = !cliente.Activo;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado actualizado. Ahora está {(cliente.Activo ? "activo" : "inactivo")}." });
        }

        // GET: api/clientes/{id}/precios
        // Devuelve los precios asignados a un cliente específico
        //[HttpGet("{id}/precios")]
        //public async Task<ActionResult<IEnumerable<PrecioClienteDetalleDto>>> GetPreciosPorCliente(int id)
        //{
        //    var cliente = await _context.Clientes.FindAsync(id);
        //    if (cliente == null)
        //        return NotFound("Cliente no encontrado.");

        //    var precios = await _context.PrecioCliente
        //        .Where(p => p.IdCliente == id)
        //        .Include(p => p.Producto)
        //        .Include(p => p.Cliente)
        //        .Select(p => new PrecioClienteDetalleDto
        //        {
        //            Cliente = p.Cliente.Nombre,
        //            Producto = p.Producto.Nombre,
        //            Precio = p.Precio,
        //            FechaRegistro = p.FechaRegistro
        //        })
        //        .ToListAsync();

        //    return Ok(precios);
        //}

    }
}
