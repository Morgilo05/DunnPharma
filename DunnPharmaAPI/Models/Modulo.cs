using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    // Esta clase representa la tabla Modulos en la base de datos
    [Table("Modulos")]
    public class Modulo
    {
        // Clave primaria con autoincremento
        [Key]
        public int IdModulo { get; set; }

        // Nombre del módulo (requerido y con máximo de 100 caracteres)
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        // Fecha de registro del módulo (automática)
        public DateTime FechaRegistro { get; set; }
    }
}
