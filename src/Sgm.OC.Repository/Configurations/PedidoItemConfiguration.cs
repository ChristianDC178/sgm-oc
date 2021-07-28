using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{

    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {

            builder.ToTable("PedidoItem");

            builder.HasKey(um => um.Id);
            builder.Property(um => um.Id).HasColumnName("PedidoItemId");
            builder.Ignore(p => p.Descripcion);

        }
    }

}
