using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Models;
using System.Collections.Generic;
using DunnPharma.API.Models;
using DunnPharma.API.DTOs;

namespace DunnPharmaAPI.Data
{
    // Esta clase representa el contexto de base de datos de Entity Framework.
    // Se utiliza para consultar y guardar datos en la base de datos SQL Server.
    public class DunnPharmaDbContext : DbContext
    {
        // Constructor que recibe opciones como la cadena de conexión
        public DunnPharmaDbContext(DbContextOptions<DunnPharmaDbContext> options)
            : base(options)
        {
        }

        // A continuación se definen los DbSet, que representan las tablas de la base de datos.
        public DbSet<Laboratorio> Laboratorios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<DireccionEnvio> DireccionesEnvio { get; set; }
        public DbSet<ListaPrecio> ListasPrecio { get; set; }
        public DbSet<PrecioCliente> PrecioCliente { get; set; } // nombre singular como propiedad
       
        public DbSet<Cardex> Cardex { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalle { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<PermisoModulo> PermisoModulo { get; set; }
          
        public DbSet<Surtido> Surtidos { get; set; }
        public DbSet<InventarioItemRaw> InventarioItems { get; set; }
        public DbSet<LoteRaw> LotesPorProducto { get; set; }
        public DbSet<CardexItemDto> CardexItems { get; set; } // DTO para resultados de Cardex

        public DbSet<PedidoListadoDto> PedidosListados { get; set; } // DTO para resultados de pedidos listados




        // Aquí se puede personalizar la creación del modelo con reglas adicionales si se requiere
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InventarioItemRaw>().HasNoKey();
            modelBuilder.Entity<LoteRaw>().HasNoKey();
            modelBuilder.Entity<CardexItemDto>().HasNoKey();
            modelBuilder.Entity<PedidoListadoDto>().HasNoKey();
        }
    }
}
