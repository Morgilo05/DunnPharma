using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Lote")]
    public class Lote
    {
        [Key]
        public int IdLote { get; set; }

        public int IdProducto { get; set; }

        public int Piezas { get; set; }

        public string CodigoLote { get; set; }

        public decimal Costo { get; set; }

        public DateTime FechaCaducidad { get; set; }

        public string Factura { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioRegistro { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}
