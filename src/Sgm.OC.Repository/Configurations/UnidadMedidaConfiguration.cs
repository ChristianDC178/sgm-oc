using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{
    public class UnidadMedidaConfiguration : IEntityTypeConfiguration<UnidadMedida>
    {

        public void Configure(EntityTypeBuilder<UnidadMedida> builder)
        {

            builder.ToTable("UnidadMedida");

            builder.HasKey(um => um.Id);
            builder.Property(um => um.Id).HasColumnName("UnidadMedidaId");
            builder.Property(p => p.Descripcion).HasColumnType("varchar(50)");

        }

    }

}
