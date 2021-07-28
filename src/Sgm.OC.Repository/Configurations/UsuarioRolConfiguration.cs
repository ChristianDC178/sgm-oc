using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class UsuarioRolConfiguration: IEntityTypeConfiguration<UsuarioRol>
    {
        public void Configure(EntityTypeBuilder<UsuarioRol> builder)
        {

            builder.ToTable("UsuarioRol");
            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.Id).HasColumnName("UsuarioRolId");
            builder.ExcludeDescriptionProperty();          

        }
    }
}
