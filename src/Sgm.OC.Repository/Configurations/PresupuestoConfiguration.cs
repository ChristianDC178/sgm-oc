using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class PresupuestoConfiguration : IEntityTypeConfiguration<Presupuesto>
    {
        public void Configure(EntityTypeBuilder<Presupuesto> builder)
        {

            builder.ToEntityTableName();
            builder.UseEntityNameAsId();

            builder.ExcludeDescriptionProperty();

            var archivoBuilder = builder.OwnsOne(p => p.Archivo);
            archivoBuilder.Property(a => a.ArchivoId).HasColumnName("ArchivoId");
            archivoBuilder.Property(a => a.Nombre).HasColumnName("Nombre");
            archivoBuilder.Property(a => a.Path).HasColumnName("Path");
        }

    }
}
