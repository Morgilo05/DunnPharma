using AutoMapper;
using DunnPharmaAPI.DTOs;
using DunnPharmaAPI.Models;

namespace DunnPharmaAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeos para Laboratorio
            CreateMap<Laboratorio, LaboratorioDto>().ReverseMap();

            // Mapeos para Cliente
            CreateMap<Cliente, ClienteDto>().ReverseMap();

            // Mapeos para Lista de Precios
            CreateMap<ListaPrecio, ListaPrecioDto>();
            CreateMap<CrearListaPrecioDto, ListaPrecio>();
            CreateMap<EditarListaPrecioDto, ListaPrecio>();

            // ✅ --- MAPEO CORRECTO Y ÚNICO PARA PRODUCTO ---

            // De Producto (Entidad) a ProductoDto (para respuestas de la API)
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.LaboratorioNombre, opt => opt.MapFrom(src => src.Laboratorio.Nombre));
            // NOTA: 'Existencia', 'Nombre', 'Costo', etc., se mapean automáticamente
            // porque tienen el mismo nombre en ambas clases. No es necesario añadir nada más.

            // De ProductoDto (para peticiones a la API) a Producto (Entidad)
            CreateMap<ProductoDto, Producto>()
                .ForMember(dest => dest.Laboratorio, opt => opt.Ignore()); // Ignoramos la propiedad de navegación para evitar errores
        }
    }
}