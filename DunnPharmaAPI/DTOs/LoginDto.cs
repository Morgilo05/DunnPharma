namespace DunnPharmaAPI.DTOs
{
    public class LoginDto
    {
        public string Nombre { get; set; }     // Usuario
        public string Contrasena { get; set; } // Contraseña sin encriptar
    }
}
