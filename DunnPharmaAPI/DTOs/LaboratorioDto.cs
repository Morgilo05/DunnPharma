namespace DunnPharmaAPI.DTOs
{
    // DTO para transferir datos del laboratorio entre cliente y servidor
    public class LaboratorioDto
    {
        public int? IdLaboratorio { get; set; }         // Se usa en edición; es opcional al crear
        public string Nombre { get; set; }              // Nombre del laboratorio (obligatorio)
        public string RazonSocial { get; set; }         // Razón social del laboratorio
    }
}
