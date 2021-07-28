using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {

            builder.ToTable("Pedido");

            builder.HasKey(um => um.Id);
            builder.Property(um => um.Id).HasColumnName("PedidoId");
            builder.HasOne(p => p.UsuarioModificacion).WithOne(u => u.Pedido);
            builder.Ignore(p => p.Descripcion);
            builder.Ignore(p => p.Rechazado);
            builder.Ignore(p => p.PrefijoId);
        }
    }

}
