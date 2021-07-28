using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Repository.Configurations
{

    public class RequisicionItemConfig : IEntityTypeConfiguration<RequisicionItem>
    {
        public void Configure(EntityTypeBuilder<RequisicionItem> builder)
        {

            builder.ToTable("RequisicionItem");

            builder.HasKey(ri => ri.Id);
            builder.Property(ri => ri.Id).HasColumnName("RequisicionItemId");
            builder.Ignore(ri => ri.Descripcion);
        }
    }

}
