using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.DTOs;
using DunnPharmaAPI.Models;

namespace DunnPharmaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermisoModuloController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public PermisoModuloController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/permisomodulo/rol/2
        // Obtiene todos los permisos de módulos asignados a un rol
        [HttpGet("rol/{idRol}")]
        public async Task<ActionResult<IEnumerable<PermisoModuloDto>>> ObtenerPorRol(int idRol)
        {
            var permisos = await _context.PermisoModulo
                .Where(p => p.IdRol == idRol)
                .ToListAsync();

            var resultado = permisos.Select(p => new PermisoModuloDto
            {
                IdRol = p.IdRol,
                IdModulo = p.IdModulo,
                TieneAcceso = p.TieneAcceso
            });

            return Ok(resultado);
        }

        // POST: api/permisomodulo
        // Crea o actualiza un permiso de módulo para un rol
        [HttpPost]
        public async Task<IActionResult> AsignarPermiso([FromBody] PermisoModuloDto dto)
        {
            // Verificar si ya existe el permiso
            var permiso = await _context.PermisoModulo
                .FirstOrDefaultAsync(p => p.IdRol == dto.IdRol && p.IdModulo == dto.IdModulo);

            if (permiso == null)
            {
                // Crear nuevo permiso
                permiso = new PermisoModulo
                {
                    IdRol = dto.IdRol,
                    IdModulo = dto.IdModulo,
                    TieneAcceso = dto.TieneAcceso,
                    FechaRegistro = DateTime.Now
                };
                _context.PermisoModulo.Add(permiso);
            }
            else
            {
                // Actualizar permiso existente
                permiso.TieneAcceso = dto.TieneAcceso;
                // permiso.FechaRegistro permanece igual o se puede actualizar si se desea
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Permiso actualizado correctamente." });
        }

        [HttpGet("rol/{idRol}/modulos")]
        public async Task<ActionResult<IEnumerable<PermisoModuloResumenDto>>> ObtenerResumenPermisos(int idRol)
        {
            // Traer todos los módulos y combinar con los permisos actuales del rol
            var modulos = await _context.Modulos
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            var permisos = await _context.PermisoModulo
                .Where(p => p.IdRol == idRol)
                .ToListAsync();

            var resultado = modulos.Select(m => new PermisoModuloResumenDto
            {
                IdModulo = m.IdModulo,
                Nombre = m.Nombre,
                TieneAcceso = permisos.Any(p => p.IdModulo == m.IdModulo && p.TieneAcceso)
            }).ToList();

            return Ok(resultado);
        }


    }


}
 