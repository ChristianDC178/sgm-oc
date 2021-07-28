using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {

            builder.ToTable("Producto");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ProductoId");
            builder.Property(p => p.Descripcion).HasColumnType("varchar(50)");

        }
    }

}
