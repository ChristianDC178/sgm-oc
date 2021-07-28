using System.Collections.Generic;

namespace Sgm.OC.Models.Requests
{
    public class ApprovePresupuestoRequest
    {

        public List<RequisicionItemPrecioRequest> Items { get; set; } = new List<RequisicionItemPrecioRequest>();
        public string ComentarioAprobacion { get; set; }
        public int SucursalAEntregar { get; set; }
    }

    public class RequisicionItemPrecioRequest
    {
        public int RequisicionItemId { get; set; }
        public decimal Precio { get; set; }
    }
}
