using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;

namespace DunnPharmaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public MovimientosController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // POST: api/movimientos/entrada
        [HttpPost("entrada")]
        public async Task<ActionResult> RegistrarEntrada([FromBody] EntradaProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.IdProducto);
            if (producto == null)
                return NotFound("El producto especificado no existe.");

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

            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync();

            var entrada = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = 'E',
                Piezas = dto.Piezas,
                IdLote = lote.IdLote,
                Costo = dto.Costo,
                Factura = dto.Factura,
                FechaMovimiento = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            _context.Cardex.Add(entrada);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Entrada registrada correctamente." });
        }

        // POST: api/movimientos/salida
        [HttpPost("salida")]
        public async Task<ActionResult> RegistrarSalida([FromBody] SalidaProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.IdProducto);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            var lote = await _context.Lotes.FindAsync(dto.IdLote);
            if (lote == null || lote.IdProducto != dto.IdProducto)
                return BadRequest("El lote especificado no corresponde al producto.");

            if (dto.Piezas > lote.Piezas)
                return BadRequest("No hay suficientes piezas en el lote.");

            lote.Piezas -= dto.Piezas;

            var salida = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = dto.Movimiento[0],
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

        // GET: api/movimientos/historial
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

            if (idProducto.HasValue)
                query = query.Where(c => c.IdProducto == idProducto);

            if (!string.IsNullOrWhiteSpace(tipoMovimiento))
                query = query.Where(c => c.Movimiento == tipoMovimiento[0]);

            if (desde.HasValue)
                query = query.Where(c => c.FechaMovimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(c => c.FechaMovimiento <= hasta.Value);

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

        // GET: api/movimientos/inventario
        // Muestra el inventario actual por producto y lote
        [HttpGet("inventario")]
        public async Task<ActionResult<IEnumerable<InventarioDetalleDto>>> ObtenerInventario()
        {
            var inventario = await _context.Lotes
                .Include(l => l.Producto)
                .Where(l => l.Piezas > 0)
                .OrderBy(l => l.Producto.Nombre)
                .ThenBy(l => l.CodigoLote)
                .Select(l => new InventarioDetalleDto
                {
                    Producto = l.Producto.Nombre,
                    CodigoLote = l.CodigoLote,
                    Piezas = l.Piezas,
                    Costo = l.Costo,
                    FechaCaducidad = l.FechaCaducidad,
                    Factura = l.Factura
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // POST: api/movimientos/devolucion
        [HttpPost("devolucion")]
        public async Task<ActionResult> RegistrarDevolucion([FromBody] DevolucionProductoDto dto)
        {
            // Validar producto
            var producto = await _context.Productos.FindAsync(dto.IdProducto);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            // Validar cliente
            var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            // Validar lote
            var lote = await _context.Lotes.FindAsync(dto.IdLote);
            if (lote == null || lote.IdProducto != dto.IdProducto)
                return BadRequest("El lote no corresponde al producto.");

            // Verificar que existan piezas en el pedido que permitan devolución
            var piezasVendidas = await _context.Cardex
                .Where(c =>
                    c.IdProducto == dto.IdProducto &&
                    c.IdLote == dto.IdLote &&
                    c.Movimiento == 'S' &&
                    c.IdCliente == dto.IdCliente &&
                    c.IdPedido == dto.IdPedido)
                .SumAsync(c => (int?)c.Piezas) ?? 0;

            var piezasDevueltas = await _context.Cardex
                .Where(c =>
                    c.IdProducto == dto.IdProducto &&
                    c.IdLote == dto.IdLote &&
                    c.Movimiento == 'D' &&
                    c.IdCliente == dto.IdCliente &&
                    c.IdPedido == dto.IdPedido)
                .SumAsync(c => (int?)c.Piezas) ?? 0;

            if ((piezasDevueltas + dto.Piezas) > piezasVendidas)
                return BadRequest("Las piezas a devolver exceden las piezas vendidas.");

            // Aumentar piezas al lote
            lote.Piezas += dto.Piezas;

            // Registrar movimiento en Cardex
            var devolucion = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = 'D',
                Piezas = dto.Piezas,
                IdLote = dto.IdLote,
                IdCliente = dto.IdCliente,
                IdPedido = dto.IdPedido,
                Costo = dto.Costo,
                PrecioVenta = dto.PrecioVenta,
                Factura = dto.Factura,
                Nota = dto.Nota,
                FechaMovimiento = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            _context.Cardex.Add(devolucion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Devolución registrada correctamente." });
        }
        // POST: api/movimientos/surtido
        [HttpPost("surtido")]
        public async Task<ActionResult> SurtirProducto([FromBody] SurtidoProductoDto dto)
        {
            // Validar pedido
            var pedido = await _context.Pedidos.FindAsync(dto.IdPedido);
            if (pedido == null)
                return NotFound("Pedido no encontrado.");

            // Validar lote
            var lote = await _context.Lotes.FindAsync(dto.IdLote);
            if (lote == null || lote.IdProducto != dto.IdProducto)
                return BadRequest("El lote no corresponde al producto.");

            if (dto.Piezas > lote.Piezas)
                return BadRequest("No hay suficientes piezas en el lote.");

            // Buscar detalle del producto en el pedido
            var detalle = await _context.PedidoDetalle
                .FirstOrDefaultAsync(d => d.IdPedido == dto.IdPedido && d.IdProducto == dto.IdProducto);

            if (detalle == null)
                return BadRequest("El producto no está registrado en el pedido.");

            // Validar que no se excedan las piezas solicitadas
            if (detalle.PiezasSurtidas + dto.Piezas > detalle.Piezas)
                return BadRequest("Se excede la cantidad de piezas solicitadas.");

            // Restar piezas al lote
            lote.Piezas -= dto.Piezas;

            // Sumar piezas surtidas al pedido
            detalle.PiezasSurtidas += dto.Piezas;

            // Registrar en Cardex
            var salida = new Cardex
            {
                IdProducto = dto.IdProducto,
                Movimiento = 'S',
                Piezas = dto.Piezas,
                IdLote = dto.IdLote,
                IdPedido = dto.IdPedido,
                IdCliente = pedido.IdCliente,
                Costo = dto.Costo,
                PrecioVenta = dto.PrecioVenta,
                Factura = dto.Factura,
                FechaMovimiento = DateTime.Now,
                UsuarioRegistro = "admin"
            };

            _context.Cardex.Add(salida);
            await _context.SaveChangesAsync();

            // Actualizar totales del pedido
            var detalles = await _context.PedidoDetalle
                .Where(d => d.IdPedido == dto.IdPedido)
                .ToListAsync();

            var surtidoCompleto = detalles.All(d => d.PiezasSurtidas >= d.Piezas);

            pedido.CostoTotal = await _context.Cardex
    .Where(c => c.IdPedido == dto.IdPedido && c.Movimiento == 'S')
    .SumAsync(c => (c.Costo ?? 0) * c.Piezas);

            pedido.Total = await _context.Cardex
                .Where(c => c.IdPedido == dto.IdPedido && c.Movimiento == 'S')
                .SumAsync(c => (c.PrecioVenta ?? 0) * c.Piezas);


            pedido.UtilidadTotal = pedido.Total - pedido.CostoTotal;
            pedido.Estatus = surtidoCompleto ? "Surtido" : "Parcial";

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto surtido correctamente." });
        }


    }
}
