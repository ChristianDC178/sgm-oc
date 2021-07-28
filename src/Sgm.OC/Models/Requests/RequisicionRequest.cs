using System.Collections.Generic;

namespace Sgm.OC.Models.Requests
{

    public class RequisicionRequest
    {
        public bool Recurrente { get; set; }
        public List<RequisicionItemRequest> Items { get; set; } = new List<RequisicionItemRequest>();
    }

    public class RequisicionItemRequest
    {
        public int ProductoId { get; set; }
        public int[] PedidoItemIds { get; set; }
        public int Cantidad { get; set; }
        public bool EnUnidades { get; set; }
    }

}
