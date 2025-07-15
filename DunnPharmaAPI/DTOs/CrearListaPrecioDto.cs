using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    public class CrearListaPrecioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Range(0, 999.99, ErrorMessage = "El porcentaje debe estar entre 0 y 999.99.")]
        public decimal PorcentajeAumento { get; set; }
    }
}