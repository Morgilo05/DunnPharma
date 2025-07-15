// DTOs/CrearProductoDto.cs
namespace DunnPharmaAPI.DTOs
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public int IdLaboratorio { get; set; }
    }
}