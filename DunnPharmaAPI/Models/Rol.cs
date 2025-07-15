using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Rol")]
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; } = true;
    }
}
