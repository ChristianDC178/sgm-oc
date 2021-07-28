using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Repository.Configurations
{
    class RubroConfiguration : IEntityTypeConfiguration<Rubro>
    {
        public void Configure(EntityTypeBuilder<Rubro> builder)
        {   
            builder.ToTable("Rubro");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("RubroId");
        }
    }
}
