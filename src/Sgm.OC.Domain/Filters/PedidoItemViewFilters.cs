using System;
using System.Linq;

namespace Sgm.OC.Domain.Filters
{

    public class PedidoItemViewFilters
    {
        public int? ProductoIdInterno { get; set; }
        public string ProductoDescripcion { get; set; }
        public int? RubroIdInterno { get; set; }
        public string RubroDescripcion { get; set; }
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string NotPedidoItemIds { get; set; }
        public bool? Recurrente { get; set; }
        public int? RequisicionId { get; set; }
        public int? EstadoRequisicion { get; set; }
        public DateTime? FechaDesdeRequisicion { get; set; }
        public DateTime? FechaHastaRequisicion { get; set; }
        public bool? RequisicionAsignada { get; set; }
    }

}
