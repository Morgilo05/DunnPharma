// ==========[ DunnPharma.API/Controllers/InventarioController.cs ]==========

using DunnPharma.API.DTOs;
using DunnPharmaAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DunnPharma.API.Controllers
{
    [Authorize] // Protege el controlador, solo usuarios autenticados pueden acceder.
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public InventarioController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/inventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioLaboratorioDto>>> GetInventario()
        {
            // Ejecuta el procedimiento almacenado que creamos.
            var inventarioRaw = await _context.InventarioItems
                .FromSqlRaw("EXEC sp_GetInventarioCompleto")
                .ToListAsync();

            // Agrupa los resultados usando LINQ para crear la estructura jerárquica deseada.
            var inventarioAgrupado = inventarioRaw
                .GroupBy(i => i.NombreLaboratorio) // 1. Agrupar por Laboratorio
                .Select(gLab => new InventarioLaboratorioDto
                {
                    NombreLaboratorio = gLab.Key,
                    Productos = gLab
                        .GroupBy(p => new { p.IdProducto, p.NombreProducto, p.TotalPiezas }) // 2. Agrupar por Producto
                        .Select(gProd => new InventarioProductoDto
                        {
                            IdProducto = gProd.Key.IdProducto,
                            NombreProducto = gProd.Key.NombreProducto,
                            TotalPiezas = gProd.Key.TotalPiezas,
                            Lotes = gProd.Select(l => new InventarioLoteDto // 3. Crear la lista de lotes
                            {
                                IdLote = l.IdLote,
                                CodigoLote = l.CodigoLote,
                                PiezasPorLote = l.PiezasPorLote,
                                Costo = l.Costo,
                                FechaCaducidad = l.FechaCaducidad,
                                FechaEntrada = l.FechaEntrada,
                                UsuarioRegistro = l.UsuarioRegistro,
                                Factura = l.Factura
                            }).ToList()
                        }).ToList()
                }).ToList();

            return Ok(inventarioAgrupado);
        }
    }
}