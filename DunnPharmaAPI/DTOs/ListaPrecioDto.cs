namespace DunnPharmaAPI.DTOs
{
    public class ListaPrecioDto
    {
        public int IdLista { get; set; }
        public string Nombre { get; set; }
        public decimal PorcentajeAumento { get; set; }
        public bool Activo { get; set; }
    }
}