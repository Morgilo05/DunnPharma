// En API: DTOs/EditarProductoDto.cs
namespace DunnPharmaAPI.DTOs
{
    public class EditarProductoDto
    {
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public int IdLaboratorio { get; set; }
    }
}