using AutoMapper;
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
    public class ListasPreciosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;
        private readonly IMapper _mapper;

        public ListasPreciosController(DunnPharmaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ListasPrecios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaPrecioDto>>> GetListasPrecios()
        {
            var listas = await _context.ListasPrecio.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ListaPrecioDto>>(listas));
        }

        // GET: api/ListasPrecios/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ListaPrecioDto>>> GetListasPreciosActivas()
        {
            var listas = await _context.ListasPrecio.Where(l => l.Activo).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ListaPrecioDto>>(listas));
        }

        // POST: api/ListasPrecios
        [HttpPost]
        public async Task<ActionResult<ListaPrecioDto>> CreateListaPrecio([FromBody] CrearListaPrecioDto crearDto)
        {
            // Validación para evitar duplicados
            if (await _context.ListasPrecio.AnyAsync(l => l.Nombre.ToLower() == crearDto.Nombre.ToLower()))
            {
                return BadRequest("Ya existe una lista de precios con ese nombre.");
            }

            var listaPrecio = _mapper.Map<ListaPrecio>(crearDto);
            listaPrecio.Activo = true;
            listaPrecio.FechaRegistro = DateTime.UtcNow;
            listaPrecio.UsuarioRegistro = "admin"; // Reemplazar con el usuario de la sesión

            _context.ListasPrecio.Add(listaPrecio);
            await _context.SaveChangesAsync();

            var listaPrecioDto = _mapper.Map<ListaPrecioDto>(listaPrecio);

            return CreatedAtAction(nameof(GetListasPrecios), new { id = listaPrecio.IdLista }, listaPrecioDto);
        }

        // PUT: api/ListasPrecios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListaPrecio(int id, [FromBody] CrearListaPrecioDto editarDto)
        {
            var listaPrecio = await _context.ListasPrecio.FindAsync(id);
            if (listaPrecio == null)
            {
                return NotFound();
            }

            // Validación de duplicados al editar
            if (await _context.ListasPrecio.AnyAsync(l => l.Nombre.ToLower() == editarDto.Nombre.ToLower() && l.IdLista != id))
            {
                return BadRequest("Ya existe otra lista de precios con ese nombre.");
            }

            _mapper.Map(editarDto, listaPrecio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/ListasPrecios/5/cambiar-estado
        [HttpPatch("{id}/cambiar-estado")]
        public async Task<IActionResult> ChangeListaPrecioStatus(int id)
        {
            var listaPrecio = await _context.ListasPrecio.FindAsync(id);
            if (listaPrecio == null)
            {
                return NotFound();
            }

            listaPrecio.Activo = !listaPrecio.Activo;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Estado actualizado correctamente." });
        }
    }
}