using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Views;

namespace Sgm.OC.Repository.Configurations
{
    public class RequisicionViewConfiguration : IEntityTypeConfiguration<RequisicionView>
    {
        public void Configure(EntityTypeBuilder<RequisicionView> builder)
        {
            builder.ToView("vw_Requisicion");
            builder.HasNoKey();
            builder.Ignore(rv => rv.CreacionFormated);
            builder.Ignore(rv => rv.ModificacionFormated);
            builder.Ignore(rv => rv.NroOC);
            builder.Ignore(p => p.PrefijoId);
            //The views doesn't accept Complex types
            //There is an exception with the Complex Type Mapping: object reference
            //Possible entity framework bug
            //builder.OwnsOne(r => r.OrdenCompraKey);
        }

    }
}
