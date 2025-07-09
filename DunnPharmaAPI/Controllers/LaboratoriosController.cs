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
    public class LaboratoriosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;
        private readonly IMapper _mapper;

        // Inyectamos el DbContext y AutoMapper
        public LaboratoriosController(DunnPharmaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/laboratorios
        // Lista todos los laboratorios (activos e inactivos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LaboratorioDto>>> GetAll()
        {
            var entidades = await _context.Laboratorios.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<LaboratorioDto>>(entidades));
        }

        // GET: api/laboratorios/activos
        // Lista solo los laboratorios activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<LaboratorioDto>>> GetActivos()
        {
            var activos = await _context.Laboratorios
                                        .Where(l => l.Activo)
                                        .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<LaboratorioDto>>(activos));
        }

        // POST: api/laboratorios
        // Crear un nuevo laboratorio si no existe uno con el mismo nombre
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] LaboratorioDto dto)
        {
            // Validar duplicado
            bool existe = await _context.Laboratorios
                .AnyAsync(l => l.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existe)
                return BadRequest("Ya existe un laboratorio con ese nombre.");

            var entidad = _mapper.Map<Laboratorio>(dto);
            entidad.FechaRegistro = DateTime.Now;
            entidad.UsuarioRegistro = "admin"; // ⚠️ Reemplazar con usuario autenticado en producción

            _context.Laboratorios.Add(entidad);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Laboratorio registrado correctamente." });
        }

        // PUT: api/laboratorios/{id}
        // Editar un laboratorio existente
        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] LaboratorioDto dto)
        {
            var entidad = await _context.Laboratorios.FindAsync(id);
            if (entidad == null)
                return NotFound("Laboratorio no encontrado.");

            // Validar que no haya otro con el mismo nombre
            bool duplicado = await _context.Laboratorios
                .AnyAsync(l => l.Nombre.ToLower() == dto.Nombre.ToLower() && l.IdLaboratorio != id);

            if (duplicado)
                return BadRequest("Ya existe otro laboratorio con ese nombre.");

            entidad.Nombre = dto.Nombre;
            entidad.RazonSocial = dto.RazonSocial;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Laboratorio actualizado correctamente." });
        }

        // PATCH: api/laboratorios/{id}/cambiar-estado
        // Cambia el estado Activo del laboratorio (true ⇄ false)
        [HttpPatch("{id}/cambiar-estado")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            var entidad = await _context.Laboratorios.FindAsync(id);
            if (entidad == null)
                return NotFound("Laboratorio no encontrado.");

            entidad.Activo = !entidad.Activo;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado actualizado. Ahora está {(entidad.Activo ? "activo" : "inactivo")}." });
        }
    }
}
