namespace DunnPharmaAPI.DTOs
{
    public class PrecioClienteDto
    {
        public int IdPrecioCliente { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public decimal Precio { get; set; }
    }
}