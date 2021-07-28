using System.Collections.Generic;

namespace Sgm.OC.Models.Requests
{

    public class PedidoRequest
    {
        public bool Recurrente { get; set; }
        public List<PedidoItemRequest> Items { get; set; } = new List<PedidoItemRequest>();
    }

    public class PedidoItemRequest
    {

        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public bool EnUnidades { get; set; }
        public int PedidoItemId { get; set; }

    }

}
