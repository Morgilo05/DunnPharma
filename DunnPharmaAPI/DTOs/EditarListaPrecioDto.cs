using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    /// <summary>
    /// DTO para recibir los datos al actualizar una Lista de Precios existente.
    /// </summary>
    public class EditarListaPrecioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Range(0, 999.99, ErrorMessage = "El porcentaje debe estar entre 0 y 999.99.")]
        public decimal PorcentajeAumento { get; set; }
    }
}