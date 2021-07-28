using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain;

namespace Sgm.OC.Repository.Configurations
{
    public class ConsolidacionPendienteConfiguration : IEntityTypeConfiguration<ConsolidacionPendiente>
    {
        public void Configure(EntityTypeBuilder<ConsolidacionPendiente> builder)
        {
            builder.ToView("vm_ConsolidacionesPendientes");
            builder.HasNoKey();
        }
    }

}

