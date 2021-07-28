using System.Net.NetworkInformation;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class RequisicionItem : EntityBase
    {

        private Requisicion _requisicion;
        private Producto _producto;

        private RequisicionItem() { }

        public RequisicionItem(int productoId, int cantidad, bool enUnidades)
        {
            ProductoId = productoId;
            Cantidad = cantidad;
            EnUnidades = enUnidades;
        }

        public int ProductoId { get; private set; }

        public Producto Producto
        {
            get { return _producto; }
            set
            {
                ProductoId = value.Id;
                _producto = value;
                Precio = value.Precio;

            }
        }

        public int Cantidad { get; set; }

        public int RequisicionId { get; private set; }

        public Requisicion Requisicion
        {
            get { return _requisicion; }
            set
            {
                RequisicionId = value.Id;
                _requisicion = value;
            }
        }

        public decimal? Precio { get; set; }
        public bool EnUnidades { get; set; }
        //public decimal PrecioSgm { get; set; }
        //public bool PreciosIguales { get; set; }
    }

}
