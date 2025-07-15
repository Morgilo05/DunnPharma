using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;
using Microsoft.AspNetCore.Authorization; // <-- AÑADE ESTE USING
using Microsoft.AspNetCore.Mvc;

namespace DunnPharmaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public RolesController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> ObtenerRoles()
        {
            var roles = await _context.Roles
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return Ok(roles);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> CrearRol([FromBody] CrearRolDto dto)
        {
            bool existe = await _context.Roles
                .AnyAsync(r => r.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existe)
                return BadRequest("Ya existe un rol con ese nombre.");

            var nuevoRol = new Rol
            {
                Nombre = dto.Nombre,
                Activo = true
            };

            _context.Roles.Add(nuevoRol);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol creado correctamente", idRol = nuevoRol.IdRol });
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarRol(int id, [FromBody] CrearRolDto dto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound("Rol no encontrado.");

            bool duplicado = await _context.Roles
                .AnyAsync(r => r.Nombre.ToLower() == dto.Nombre.ToLower() && r.IdRol != id);

            if (duplicado)
                return BadRequest("Ya existe otro rol con ese nombre.");

            rol.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol actualizado correctamente." });
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound("Rol no encontrado.");

            // Eliminación lógica
            rol.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol eliminado correctamente (inactivo)." });
        }
    }
}
