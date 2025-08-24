using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YastCleaner.Entities.Entidades;

namespace YastCleaner.Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Cliente> TblCliente { get; set; }
        public DbSet<DetallePedido> TblDetallePedido { get; set; }
        public DbSet<Pedido> TblPedido { get; set; }
        public DbSet<PedidoAnulado> TblPedidoAnulado { get; set; }
        public DbSet<PedidoEntregado> TblPedidoEntregado { get; set; }
        public DbSet<Reporte> TblReporte { get; set;}
        public DbSet<Servicio> TblServicio { get; set; }
        public DbSet<Usuario> TblUsuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasConversion<string>();

            modelBuilder.Entity<Cliente>()
                .Property(u => u.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(u => u.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(u => u.MetodoPago)
                .HasConversion<string>();

            modelBuilder.Entity<Servicio>()
                .Property(s=>s.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.PedidoAnulado)
                .WithOne(pa => pa.Pedido)
                .HasForeignKey<PedidoAnulado>(pa => pa.PedidoId);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.PedidoEntregado)
                .WithOne(pe => pe.Pedido)
                .HasForeignKey<PedidoEntregado>(pe => pe.PedidoId);
        }

    }
}
