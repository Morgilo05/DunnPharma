using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    public class Surtido
    {
        [Key]
        public int IdSurtido { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int Piezas { get; set; }
        public int IdLote { get; set; }
        [MaxLength(100)]
        public string Factura { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Costo { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostoSubtotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Utilidad { get; set; }
        public DateTime FechaRegistro { get; set; }
        [MaxLength(50)]
        public string UsuarioRegistro { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdPedido")]
        public virtual Pedido Pedido { get; set; }
        [ForeignKey("IdProducto")]
        public virtual Producto Producto { get; set; }
        [ForeignKey("IdLote")]
        public virtual Lote Lote { get; set; }
    }
}