using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("DireccionEnvio")]
    public class DireccionEnvio
    {
        [Key]
        public int IdDireccion { get; set; }
        public int IdCliente { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; } = true;
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
    }
}
