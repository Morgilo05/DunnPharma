namespace DunnPharma.API.DTOs
{
    // DTO para enviar la información de un lote a la aplicación cliente.
    public class LoteDto
    {
        public int IdLote { get; set; }
        public string CodigoLote { get; set; }
        public int PiezasDisponibles { get; set; }
        public DateTime? FechaCaducidad { get; set; }
    }
}


// ==========[ DunnPharma.API/DTOs/RegistrarMermaDto.cs ]==========

namespace DunnPharma.API.DTOs
{
    // DTO para recibir los datos necesarios para registrar una merma.
    public class RegistrarMermaDto
    {
        public int IdProducto { get; set; }
        public int IdLote { get; set; }
        public int Piezas { get; set; }
        public string Motivo { get; set; }
    }
}