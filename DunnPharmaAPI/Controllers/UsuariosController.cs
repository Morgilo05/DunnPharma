using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.Models;
using DunnPharmaAPI.DTOs;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DunnPharmaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public UsuariosController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    IdRol = u.IdRol,
                    Activo = u.Activo,
                    FechaRegistro = u.FechaRegistro
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
        {
            // Validar duplicado
            if (await _context.Usuarios.AnyAsync(u => u.Nombre.ToLower() == dto.Nombre.ToLower()))
                return BadRequest("Ya existe un usuario con ese nombre.");

            // Encriptar contraseña
            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var nuevo = new Usuario
            {
                Nombre = dto.Nombre,
                ContrasenaHash = hash,
                IdRol = dto.IdRol,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente." });
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] CrearUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Validar que el nombre no esté duplicado
            var duplicado = await _context.Usuarios
                .AnyAsync(u => u.Nombre.ToLower() == dto.Nombre.ToLower() && u.IdUsuario != id);
            if (duplicado)
                return BadRequest("Ya existe otro usuario con ese nombre.");

            usuario.Nombre = dto.Nombre;
            usuario.IdRol = dto.IdRol;

            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // PUT: api/usuarios/activar/{id}
        [HttpPut("activar/{id}")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.Activo = !usuario.Activo;
            await _context.SaveChangesAsync();

            var estado = usuario.Activo ? "activado" : "desactivado";
            return Ok(new { mensaje = $"Usuario {estado} correctamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Buscar el usuario por nombre
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == dto.Nombre);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado.");

            // Verificar contraseña con BCrypt
            bool esValida = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);
            if (!esValida)
                return Unauthorized("Contraseña incorrecta.");

            // Generar JWT
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim("idUsuario", usuario.IdUsuario.ToString()),
        new Claim("rol", usuario.Rol.Nombre)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Duunpharma2025librojavonmonedasi")); // igual que en Program.cs
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, usuario = usuario.Nombre, rol = usuario.Rol.Nombre });
        }
    }
}
