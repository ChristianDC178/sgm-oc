using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class UsuarioConfiguration:IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(us=>us.Id);
            builder.Property(us => us.Id).HasColumnName("UsuarioId");
            builder.ExcludeDescriptionProperty();
        }

    }
}
