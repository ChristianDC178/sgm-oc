using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain;
using Sgm.OC.Domain.WF;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Repository.Configurations
{

    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.ToEntityTableName();
            builder.HasKey(ur => ur.Id);
            builder.UseEntityNameAsId();
        }
    }

    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.ToEntityTableName();
            builder.HasKey(ur => ur.Id);
            builder.UseEntityNameAsId();
        }
    }


    public class WorkflowEstadoConfiguration : IEntityTypeConfiguration<WorkflowEstado>
    {
        public void Configure(EntityTypeBuilder<WorkflowEstado> builder)
        {
            builder.ToEntityTableName();
            builder.HasKey(ur => ur.Id);
            builder.UseEntityNameAsId();
            builder.ExcludeDescriptionProperty();
        }
    }

    public class WorkflowEstadoEntidadConfiguration : IEntityTypeConfiguration<WorkflowEstadoEntidad>
    {
        public void Configure(EntityTypeBuilder<WorkflowEstadoEntidad> builder)
        {
            builder.ToEntityTableName();
            builder.HasKey(ur => ur.Id);
            builder.UseEntityNameAsId();
            builder.ExcludeDescriptionProperty();
        }
    }

}
