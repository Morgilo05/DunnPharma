using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("PedidoDetalle")]
    public class PedidoDetalle
    {
        [Key]
        public int IdDetalle { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }
        public int Piezas { get; set; }
        public int PiezasSurtidas { get; set; }
        public DateTime FechaRegistro { get; set; }
        [MaxLength(50)]
        public string UsuarioRegistro { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdPedido")]
        public virtual Pedido Pedido { get; set; }
        [ForeignKey("IdProducto")]
        public virtual Producto Producto { get; set; }
    }
}
