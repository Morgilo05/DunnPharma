using System;

namespace DunnPharmaAPI.DTOs
{
    /// <summary>
    /// Representa el precio asignado de un producto para un cliente específico.
    /// Se retorna en el GET: api/PrecioCliente/{idCliente}
    /// </summary>
    public class PrecioClienteDetalleDto
    {
        /// <summary>Identificador del producto.</summary>
        public int IdProducto { get; set; }

        /// <summary>Nombre del producto.</summary>
        public string NombreProducto { get; set; } = string.Empty;

        /// <summary>Costo base del producto.</summary>
        public decimal CostoProducto { get; set; }

        /// <summary>Precio específico asignado a este cliente.</summary>
        public decimal PrecioAsignado { get; set; }
    }
}
