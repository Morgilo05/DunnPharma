using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    public class CrearModuloDto
    {
        [Required(ErrorMessage = "El nombre del módulo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }
    }
}
