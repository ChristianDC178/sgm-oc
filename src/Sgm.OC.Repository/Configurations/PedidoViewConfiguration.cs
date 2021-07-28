using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Repository.Configurations
{
    public class PedidoViewConfiguration : IEntityTypeConfiguration<PedidoView>
    {
        public void Configure(EntityTypeBuilder<PedidoView> builder)
        {
            builder.ToView("vw_Pedido");
            builder.HasNoKey();
            builder.Ignore(pv => pv.CreacionFormated);
            builder.Ignore(p => p.PrefijoId);
            builder.Ignore(pv => pv.ModificacionFormated);
        }
    }
}
