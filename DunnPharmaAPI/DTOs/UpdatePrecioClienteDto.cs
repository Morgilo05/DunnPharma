using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    public class UpdatePrecioClienteDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal NuevoPrecio { get; set; }
    }
}