using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {

            builder.ToTable("Proveedor");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ProveedorId");
            builder.Property(p => p.Descripcion).HasColumnType("varchar(50)");

            builder.Ignore(p => p.Creacion);
            builder.Ignore(p => p.Modificacion);

        }
    }
}
