using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
    {
        public void Configure(EntityTypeBuilder<Sucursal> builder)
        {

            builder.ToEntityTableName();

            builder.ToTable("Sucursal");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("SucursalId");
            builder.ConfigureDescriptionProperty();

        }
    }

}
