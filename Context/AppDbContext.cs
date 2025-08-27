using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Detalle_Permiso> Detalle_Permisos { get; set; }
        public DbSet<Detalle_Venta> Detalle_Ventas { get; set; }
        public DbSet<Detalle_Temp> Detalle_Temp { get; set; }
        public DbSet<Laboratorio> Laboratorios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Presentacion> Presentaciones { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Tipo> Tipos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
    }
}
