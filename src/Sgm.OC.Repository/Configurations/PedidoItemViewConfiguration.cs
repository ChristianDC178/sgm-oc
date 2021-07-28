using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Views;

namespace Sgm.OC.Repository.Configurations
{
    public class PedidoItemViewConfiguration : IEntityTypeConfiguration<PedidoItemView>
    {
        public void Configure(EntityTypeBuilder<PedidoItemView> builder)
        {
            builder.ToView("vw_PedidoItems");
            builder.Ignore(piv => piv.CreacionRequisicionFormatted);
            builder.HasNoKey();
        }
    }
}
