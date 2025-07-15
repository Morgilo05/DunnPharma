namespace DunnPharmaAPI.DTOs
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int IdRol { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
