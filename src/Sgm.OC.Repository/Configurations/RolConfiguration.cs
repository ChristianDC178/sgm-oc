using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class RolConfiguration:IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {

            builder.ToTable("Rol");
            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.Id).HasColumnName("RolId");
            builder.ConfigureDescriptionProperty(50);

        }

    }

}
