using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DunnPharmaAPI.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        public string Nombre { get; set; }

        public int IdLista { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioRegistro { get; set; }

        public bool Activo { get; set; } = true;

        [ForeignKey("IdLista")] // ?? Esta línea es la clave
        public ListaPrecio ListaPrecio { get; set; }
    }
}
