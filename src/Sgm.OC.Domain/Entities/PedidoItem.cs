using System.Net.NetworkInformation;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class PedidoItem : EntityBase
    {

        private Pedido _pedido;
        private Producto _producto;
        private Requisicion _requisicion;

        private PedidoItem() { }

        public PedidoItem(int productoId, int cantidad, bool enUnidades)
        {
            ProductoId = productoId;
            Cantidad = cantidad;
            EnUnidades = enUnidades;
        }

        public int ProductoId { get; private set; }
        public int Cantidad { get; set; }
        public int PedidoId { get; private set; }
        public bool EnUnidades { get; set; }

        public int? RequisicionId { get; set; }

        public Pedido Pedido
        {
            get { return _pedido; }
            set
            {
                PedidoId = value.Id;
                _pedido = value;
            }
        }
   
        public Requisicion Requisicion 
        {
            get { return _requisicion; }
            set
            {
                _requisicion = value;
                RequisicionId = value.Id;
            }
        }

        public Producto Producto
        {
            get { return _producto; }
            set
            {
                ProductoId = value.Id;
                _producto = value;
            }
        }

    }

}





