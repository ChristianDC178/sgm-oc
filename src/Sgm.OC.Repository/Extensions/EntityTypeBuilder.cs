using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sgm.OC.Framework;

namespace Sgm.OC.Repository.Extensions
{
    public static class EntityTypeBuilderExtensions
    {

        public static EntityTypeBuilder UseEntityNameAsId<T>(this EntityTypeBuilder<T> builder, bool useIdentity = true) where T : EntityBase
        {

            builder.HasKey("Id");
            var propertyBuilder = builder.Property("Id").HasColumnName(typeof(T).Name + "Id");

            if (useIdentity)
                propertyBuilder.UseIdentityColumn();

            return builder;
        }

        public static EntityTypeBuilder ExcludeDescriptionProperty<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
        {
            builder.Ignore(e => e.Descripcion);
            return builder;
        }

        public static EntityTypeBuilder ConfigureDescriptionProperty<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
        {
            builder.ConfigureDescriptionProperty(50);
            return builder;
        }


        public static EntityTypeBuilder ConfigureDescriptionProperty<T>(this EntityTypeBuilder<T> builder, int length) where T : EntityBase
        {
            builder.Property(e => e.Descripcion).HasColumnType($"varchar({length})");
            return builder;
        }


        public static EntityTypeBuilder ConfigureCreacionProperty<T>(this EntityTypeBuilder<T> builder) where T : EntityBussiness
        {
            builder.Property(e => e.Creacion);
            return builder;
        }

        public static EntityTypeBuilder ConfigureModificacionProperty<T>(this EntityTypeBuilder<T> builder) where T : EntityBussiness
        {
            builder.Property(e => e.Modificacion);
            return builder;
        }


        public static EntityTypeBuilder ToEntityTableName<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
        {
            builder.ToTable(typeof(T).Name);
            return builder;
        }


    }


}
