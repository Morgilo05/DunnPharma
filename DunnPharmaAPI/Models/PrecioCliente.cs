using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("PrecioCliente")] // Asegura que mapea a la tabla correcta
    public class PrecioCliente
    {
        [Key]
        public int IdPrecio { get; set; }  // ← Usa el nombre exacto de la columna en SQL

        public int IdCliente { get; set; }         // FK a Cliente
        public int IdProducto { get; set; }        // FK a Producto

        public decimal Precio { get; set; }        // Precio asignado al cliente

        public DateTime FechaRegistro { get; set; } // Fecha de asignación

        public string UsuarioRegistro { get; set; } // Usuario que asignó

        // Navegación (opcional pero útil para joins automáticos con EF)
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}
