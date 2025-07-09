using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Cardex")]
    public class Cardex
    {
        [Key]
        public int IdCardex { get; set; }

        public int IdProducto { get; set; }

        public char Movimiento { get; set; } // 'E', 'S', 'M', 'D'

        public int Piezas { get; set; }

        public int? IdLote { get; set; }

        public int? IdPedido { get; set; }

        public int? IdCliente { get; set; }

        public decimal? Costo { get; set; }

        public decimal? PrecioVenta { get; set; }

        public string Factura { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public string Nota { get; set; }

        public string UsuarioRegistro { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }

        [ForeignKey("IdLote")]
        public Lote Lote { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
    }
}
