using System.ComponentModel.DataAnnotations;

namespace DunnPharmaAPI.DTOs
{
    /// <summary>
    /// DTO para recibir los datos al actualizar un Cliente existente.
    /// </summary>
    public class EditarClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La lista de precios es obligatoria.")]
        public int IdLista { get; set; }
    }
}