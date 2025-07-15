namespace DunnPharmaAPI.DTOs
{
    // Representa los datos necesarios para crear o editar un usuario
    public class CrearUsuarioDto
    {
        public int IdUsuario { get; set; }         // Solo se usa en edición
        public string Nombre { get; set; }         // Nombre de usuario
        public string Contrasena { get; set; }     // Contraseña en texto plano
        public int IdRol { get; set; }             // Rol asignado
    }
}
