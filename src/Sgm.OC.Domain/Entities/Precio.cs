using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Precio : EntityBase
    {

        public int ProveedorId { get; private set; }
        public int ProductoId { get; private set; }

        public Proveedor Proveedor { get; private set; }
        public Producto Producto { get; private set; }

    }

}
