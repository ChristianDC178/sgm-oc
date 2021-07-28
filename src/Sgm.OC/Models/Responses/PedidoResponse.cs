using System;
using System.Collections.Generic;

namespace Sgm.OC.Models.Responses
{

    public class PedidoResponse
    {
        public string PedidoId { get; set; }
        public string PrefijoId { get; set; }
        public bool Recurrente { get; set; }
        public string Estado { get; set; }
        public string Sucursal { get; set; }
        public string Usuario { get; set; }
        public DateTime Creacion { get; set; }
        public bool Rechazado { get; set; }
        public List<PedidoItemResponse> Items { get; set; } = new List<PedidoItemResponse>();
    }

    public class PedidoItemResponse
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public bool EnUnidades { get; set; }
        public string Descripcion { get; set; }
        public int PedidoItemId { get; set; }
        public int? FactorConversion { get; set; }
    }
}
