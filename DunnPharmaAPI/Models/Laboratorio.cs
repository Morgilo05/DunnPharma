using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Laboratorio")]
    public class Laboratorio
    {
        [Key]
        public int IdLaboratorio { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; } = true;
    }
}
