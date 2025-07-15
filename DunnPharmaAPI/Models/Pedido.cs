using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Pedido")]
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        public int IdCliente { get; set; }

        public decimal Total { get; set; }

        public decimal CostoTotal { get; set; }

        public decimal UtilidadTotal { get; set; }

        public string Estatus { get; set; }
        
        [MaxLength(50)]
        public string UsuarioRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        // 🔧 Relación uno a muchos: un pedido tiene muchos detalles
        public ICollection<PedidoDetalle> PedidoDetalle { get; set; }
    }
}
