using System;
using System.Collections.Generic;
using System.Text;
using Sgm.OC.Domain.Enums;

namespace Sgm.OC.Domain
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
