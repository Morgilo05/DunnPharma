using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;

namespace DunnPharmaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModulosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public ModulosController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/modulos
        // Devuelve todos los módulos existentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuloDto>>> ObtenerModulos()
        {
            var modulos = await _context.Modulos
                .OrderBy(m => m.Nombre)
                .Select(m => new ModuloDto
                {
                    IdModulo = m.IdModulo,
                    Nombre = m.Nombre,
                    FechaRegistro = m.FechaRegistro
                })
                .ToListAsync();

            return Ok(modulos);
        }

        // GET: api/modulos/5
        // Devuelve un módulo por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuloDto>> ObtenerModuloPorId(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);

            if (modulo == null)
                return NotFound("Módulo no encontrado.");

            var dto = new ModuloDto
            {
                IdModulo = modulo.IdModulo,
                Nombre = modulo.Nombre,
                FechaRegistro = modulo.FechaRegistro
            };

            return Ok(dto);
        }

        // POST: api/modulos
        // Registra un nuevo módulo
        [HttpPost]
        public async Task<ActionResult> CrearModulo([FromBody] CrearModuloDto dto)
        {
            // Validar existencia por nombre
            bool existe = await _context.Modulos
                .AnyAsync(m => m.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existe)
                return BadRequest("Ya existe un módulo con ese nombre.");

            var nuevoModulo = new Modulo
            {
                Nombre = dto.Nombre,
                FechaRegistro = DateTime.Now
            };

            _context.Modulos.Add(nuevoModulo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Módulo registrado correctamente.", id = nuevoModulo.IdModulo });
        }

        // PUT: api/modulos/5
        // Actualiza un módulo existente
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarModulo(int id, [FromBody] EditarModuloDto dto)
        {
            if (id != dto.IdModulo)
                return BadRequest("El ID del módulo no coincide con el de la URL.");

            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo == null)
                return NotFound("Módulo no encontrado.");

            // Validar que no haya otro con el mismo nombre
            bool duplicado = await _context.Modulos
                .AnyAsync(m => m.IdModulo != id && m.Nombre.ToLower() == dto.Nombre.ToLower());

            if (duplicado)
                return BadRequest("Ya existe otro módulo con ese nombre.");

            modulo.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Módulo actualizado correctamente." });
        }

        // DELETE: api/modulos/5
        // Elimina un módulo permanentemente
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarModulo(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo == null)
                return NotFound("Módulo no encontrado.");

            _context.Modulos.Remove(modulo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Módulo eliminado correctamente." });
        }
    }
}
