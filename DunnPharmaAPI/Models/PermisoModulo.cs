using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    // Esta clase representa la tabla PermisoModulo en la base de datos.
    [Table("PermisoModulo")]
    public class PermisoModulo
    {
        // Clave primaria
        [Key]
        public int IdPermiso { get; set; }

        // Clave foránea al rol
        [Required]
        public int IdRol { get; set; }

        // Clave foránea al módulo
        [Required]
        public int IdModulo { get; set; }

        // Indica si el rol tiene acceso al módulo
        [Required]
        public bool TieneAcceso { get; set; }

        // Fecha de registro del permiso
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación de navegación hacia la tabla Rol
        [ForeignKey("IdRol")]
        public Rol Rol { get; set; }

        // Relación de navegación hacia la tabla Modulo
        [ForeignKey("IdModulo")]
        public Modulo Modulo { get; set; }
    }
}
