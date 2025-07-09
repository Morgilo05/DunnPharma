using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("ListaPrecio")]
    public class ListaPrecio
    {
        [Key]
        public int IdLista { get; set; }
        public string Nombre { get; set; }
        public decimal PorcentajeAumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; } = true;
    }
}
