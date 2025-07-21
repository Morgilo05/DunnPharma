
using DunnPharma.API.DTOs;
using DunnPharmaAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DunnPharma.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MermasController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public MermasController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/Mermas/Lotes/{idProducto}
        [HttpGet("Lotes/{idProducto}")]
        public async Task<ActionResult<IEnumerable<LoteDto>>> GetLotesPorProducto(int idProducto)
        {
            // === INICIO DE LA CORRECCIÓN ===

            // 1. Ejecuta el SP y materializa los resultados en una lista en memoria.
            var lotesRaw = await _context.LotesPorProducto
                .FromSqlRaw("EXEC sp_GetLotesPorProducto @IdProducto", new SqlParameter("@IdProducto", idProducto))
                .ToListAsync();

            // 2. Ahora, con los datos en memoria, realiza la transformación al DTO.
            var lotesDto = lotesRaw.Select(l => new LoteDto
            {
                IdLote = l.IdLote,
                CodigoLote = l.CodigoLote,
                PiezasDisponibles = l.PiezasDisponibles,
                FechaCaducidad = l.FechaCaducidad
            }).ToList();

            // === FIN DE LA CORRECCIÓN ===

            return Ok(lotesDto);
        }

        // POST: api/Mermas
        [HttpPost]
        public async Task<IActionResult> RegistrarMerma([FromBody] RegistrarMermaDto mermaDto)
        {
            // ... (El resto del método de registrar merma no necesita cambios)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuarioRegistro = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuarioRegistro))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }
            var parameters = new[]
            {
                new SqlParameter("@IdProducto", mermaDto.IdProducto),
                new SqlParameter("@IdLote", mermaDto.IdLote),
                new SqlParameter("@Piezas", mermaDto.Piezas),
                new SqlParameter("@Motivo", mermaDto.Motivo),
                new SqlParameter("@UsuarioRegistro", usuarioRegistro)
            };
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_RegistrarSalidaMerma @IdProducto, @IdLote, @Piezas, @Motivo, @UsuarioRegistro",
                parameters);
            return Ok(new { message = "Salida por merma registrada correctamente." });
        }
    }
}