using DunnPharmaAPI.Data;
using DunnPharmaAPI.DTOs;
using DunnPharmaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public PedidosController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // POST: api/Pedidos  
        [HttpPost]
        public async Task<ActionResult> RegistrarPedido([FromBody] PedidoDetalleDto pedidoDto)
        {
            // ✅ Obtener el nombre del usuario logueado desde el token JWT  
            var usuarioActual = User.Identity?.Name ?? "sistema";
            // Usamos una transacción para asegurar que toda la operación sea atómica.  
            // Si algo falla, se revierte todo.  
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                // Log para debug
                Console.WriteLine($"=== PEDIDO RECIBIDO ===");
                Console.WriteLine($"Cliente ID: {pedidoDto.IdCliente}");
                Console.WriteLine($"Productos: {pedidoDto.Productos?.Count ?? 0}");

                if (pedidoDto.Productos != null)
                {
                    foreach (var item in pedidoDto.Productos)
                    {
                        Console.WriteLine($"  - Producto ID: {item.IdProducto}, Piezas: {item.Piezas}, Precio: {item.PrecioUnitario}");
                    }
                }

                // 1. Crear el registro principal del Pedido  
                var nuevoPedido = new Pedido
                {
                    IdCliente = pedidoDto.IdCliente,
                    FechaRegistro = DateTime.UtcNow,
                    Estatus = "Confirmado",
                    Total = 0, // Se calculará al final  
                    CostoTotal = 0,
                    UtilidadTotal = 0,
                    UsuarioRegistro = usuarioActual
                };
                _context.Set<Pedido>().Add(nuevoPedido); // Cambiar a _context.Set<Pedido>() para evitar ambigüedad  
                await _context.SaveChangesAsync(); // Guardamos para obtener el IdPedido  

                decimal costoTotalAcumulado = 0;
                decimal ventaTotalAcumulada = 0;

                // 2. Procesar cada producto del pedido  
                foreach (var item in pedidoDto.Productos)
                {
                    var producto = await _context.Productos.FindAsync(item.IdProducto);
                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"El producto con ID {item.IdProducto} no existe.");
                    }

                    // Guardar el detalle del pedido  
                    var detalle = new PedidoDetalle
                    {
                        IdPedido = nuevoPedido.IdPedido,
                        IdProducto = item.IdProducto,
                        Piezas = item.Piezas,
                        PrecioUnitario = item.PrecioUnitario,
                        PiezasSurtidas = item.Piezas, // Asumimos que se surte todo lo pedido  
                        FechaRegistro = DateTime.UtcNow,
                        UsuarioRegistro = usuarioActual
                    };
                    _context.Set<PedidoDetalle>().Add(detalle);

                    // 3. Lógica de surtido automático  
                    int piezasPorSurtir = item.Piezas;

                    // Buscamos los lotes con existencia, ordenados por la prioridad definida  
                    var lotesDisponibles = await _context.Lotes
                        .Where(l => l.IdProducto == item.IdProducto && l.Piezas > 0)
                        .OrderBy(l => l.FechaCaducidad)
                        .ThenBy(l => l.CodigoLote)
                        .ThenBy(l => l.FechaRegistro)
                        .ToListAsync();

                    foreach (var lote in lotesDisponibles)
                    {
                        if (piezasPorSurtir <= 0) break;

                        int piezasA_TomarDelLote = Math.Min(piezasPorSurtir, lote.Piezas);

                        // Crear registro de surtido  
                        var surtido = new Surtido
                        {
                            IdPedido = nuevoPedido.IdPedido,
                            IdProducto = item.IdProducto,
                            Piezas = piezasA_TomarDelLote,
                            IdLote = lote.IdLote,
                            Factura = lote.Factura,
                            Costo = lote.Costo,
                            Precio = item.PrecioUnitario,
                            Subtotal = item.PrecioUnitario * piezasA_TomarDelLote,
                            CostoSubtotal = lote.Costo * piezasA_TomarDelLote,
                            Utilidad = (item.PrecioUnitario - lote.Costo) * piezasA_TomarDelLote,
                            FechaRegistro = DateTime.UtcNow,
                            UsuarioRegistro = usuarioActual
                        };
                        _context.Surtidos.Add(surtido);

                        // Crear movimiento en Cardex  
                        var cardex = new Cardex
                        {
                            IdProducto = item.IdProducto,
                            Movimiento = 'S', // Salida por Venta  
                            Piezas = piezasA_TomarDelLote,
                            IdLote = lote.IdLote,
                            IdPedido = nuevoPedido.IdPedido,
                            IdCliente = pedidoDto.IdCliente,
                            Costo = lote.Costo,
                            PrecioVenta = item.PrecioUnitario,
                            Factura = lote.Factura,
                            FechaMovimiento = DateTime.UtcNow,
                            UsuarioRegistro = usuarioActual
                        };
                        _context.Cardex.Add(cardex);

                        // Actualizar existencias del lote  
                        lote.Piezas -= piezasA_TomarDelLote;

                        piezasPorSurtir -= piezasA_TomarDelLote;

                        costoTotalAcumulado += surtido.CostoSubtotal;
                        ventaTotalAcumulada += surtido.Subtotal;
                    }

                    if (piezasPorSurtir > 0)
                    {
                        // No hubo suficientes piezas en todos los lotes para cumplir el pedido  
                        await transaction.RollbackAsync();
                        return BadRequest($"Stock insuficiente para el producto '{producto.Nombre}'. Faltaron {piezasPorSurtir} piezas por surtir.");
                    }
                }

                // 4. Actualizar los totales en el registro principal del Pedido  
                nuevoPedido.CostoTotal = costoTotalAcumulado;
                nuevoPedido.Total = ventaTotalAcumulada;
                nuevoPedido.UtilidadTotal = ventaTotalAcumulada - costoTotalAcumulado;

                // 5. Guardar todos los cambios y confirmar la transacción  
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Pedido registrado y surtido correctamente.", idPedido = nuevoPedido.IdPedido });
            }
            catch (Exception ex)
            {

                Console.WriteLine($"ERROR EN API: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                await transaction.RollbackAsync();
                return StatusCode(500, $"Error: {ex.Message}"); // Temporalmente devolver el error real
                //await transaction.RollbackAsync();
                //// Loguear el error 'ex' en un sistema de logs  
                //return StatusCode(500, "Ocurrió un error interno al procesar el pedido.");
            }
        }
    }
}