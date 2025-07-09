namespace DunnPharmaAPI.DTOs
{
    public class ClienteDto
    {
        public int? IdCliente { get; set; }       // Se usa en edición
        public string Nombre { get; set; }        // Nombre del cliente
        public int IdLista { get; set; }          // Clave de la lista de precios asignada
    }
}
