using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{
    public class RequisicionConfig : IEntityTypeConfiguration<Requisicion>
    {
        public void Configure(EntityTypeBuilder<Requisicion> builder)
        {

            builder.ToEntityTableName();
            builder.UseEntityNameAsId();
            builder.ExcludeDescriptionProperty();

            builder.Property(r => r.CotizacionPedida).HasColumnName("CotizacionEnviada");

            var ordenCompraKey = builder.OwnsOne(r => r.OrdenCompraKey);
            ordenCompraKey.Property(ock => ock.Codigo).HasColumnName("Codigo");
            ordenCompraKey.Property(ock => ock.Sufijo).HasColumnName("Sufijo");
            ordenCompraKey.Property(ock => ock.Prefijo).HasColumnName("Prefijo");
            ordenCompraKey.Ignore(ock => ock.NroOC);
            builder.Ignore(p => p.PrefijoId);
        }
    }

}
