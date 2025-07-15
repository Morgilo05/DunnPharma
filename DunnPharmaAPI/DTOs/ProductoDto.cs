namespace DunnPharmaAPI.DTOs
{
    // DTO para recibir y enviar datos de producto desde la API
    public class ProductoDto
    {
        public int? IdProducto { get; set; }           // Se usa en edición
        public string Nombre { get; set; }             // Nombre comercial
        public int IdLaboratorio { get; set; }         // Clave foránea al laboratorio
        public decimal Costo { get; set; }             // Costo de adquisición
        public string LaboratorioNombre { get; set; }  // Nombre del laboratorio, se usa para mostrar en la UI
        public bool Activo { get; set; }               // Indica si el producto está activo o inactivo

        public int Existencia { get; set; }            //Indica la existencia del producto
    }
}
