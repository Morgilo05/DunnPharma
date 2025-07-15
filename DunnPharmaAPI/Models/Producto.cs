using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Producto")]
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public int IdLaboratorio { get; set; }
        public decimal Costo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; } = true;
        // ?? Relación explícita con Laboratorio usando ForeignKey
        [ForeignKey("IdLaboratorio")]
        public Laboratorio Laboratorio { get; set; }
        public int Existencia { get; set; }
    }
}
