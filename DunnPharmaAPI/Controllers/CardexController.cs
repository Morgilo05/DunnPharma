using DunnPharma.API.DTOs;
using DunnPharmaAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DunnPharma.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CardexController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public CardexController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // Usamos [FromQuery] para que los filtros se pasen en la URL.
        // GET: api/Cardex
        [HttpGet]
        public async Task<IActionResult> GetCardexFiltrado([FromQuery] CardexFilterDto filters)
        {
            // --- INICIO DE LA CORRECCIÓN ---
            // Se convierten los valores nulos de C# a DBNull.Value para que SQL los entienda.
            var parameters = new[]
            {
        new SqlParameter("@FechaInicio", (object)filters.FechaInicio ?? DBNull.Value),
        new SqlParameter("@FechaFin", (object)filters.FechaFin ?? DBNull.Value),
        new SqlParameter("@IdProducto", (object)filters.IdProducto ?? DBNull.Value),
        new SqlParameter("@IdCliente", (object)filters.IdCliente ?? DBNull.Value),
        new SqlParameter("@TipoMovimiento", (object)filters.TipoMovimiento ?? DBNull.Value),
        new SqlParameter("@UsuarioRegistro", (object)filters.UsuarioRegistro ?? DBNull.Value)
    };
            // --- FIN DE LA CORRECCIÓN ---

            // La entidad CardexItemDto debe estar registrada en el DbContext con .HasNoKey()
            var result = await _context.Set<CardexItemDto>()
                .FromSqlRaw("EXEC sp_GetCardexFiltrado @FechaInicio, @FechaFin, @IdProducto, @IdCliente, @TipoMovimiento, @UsuarioRegistro", parameters)
                .ToListAsync();

            return Ok(result);
        }
    }
}