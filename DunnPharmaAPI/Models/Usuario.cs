using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string ContrasenaHash { get; set; }
        public int IdRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
        [ForeignKey("IdRol")]
        public Rol Rol { get; set; }
    }
}
