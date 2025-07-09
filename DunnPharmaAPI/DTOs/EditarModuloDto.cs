using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    public class EditarModuloDto
    {
        [Required(ErrorMessage = "El ID del módulo es obligatorio.")]
        public int IdModulo { get; set; }

        [Required(ErrorMessage = "El nombre del módulo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }
    }
}
