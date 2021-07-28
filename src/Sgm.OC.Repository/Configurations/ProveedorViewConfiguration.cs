using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Repository.Configurations
{
    public class ProveedorViewConfiguration : IEntityTypeConfiguration<ProveedorView>
    {
        public void Configure(EntityTypeBuilder<ProveedorView> builder)
        {
            builder.ToView("vw_Proveedor");
            builder.HasNoKey();
        }
    }
}
