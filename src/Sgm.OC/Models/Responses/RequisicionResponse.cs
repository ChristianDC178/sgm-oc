using System;
using System.Collections.Generic;
using System.Net;

namespace Sgm.OC.Models.Responses
{

    public class RequisicionResponse
    {
        public string RequisicionId { get; set; }
        public string PrefijoId { get; set; }
        public bool Recurrente { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Sucursal { get; set; }
        public string Usuario { get; set; }
        public DateTime Creacion { get; set; }
        public bool CotizacionPedida { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string ComentarioAprobacion { get; set; }
        public string NroOC { get; set; }
        public List<RequisicionItemResponse> Items { get; set; } = new List<RequisicionItemResponse>();
        public string? SucursalAEntregar { get; set; }
    }

    public class RequisicionItemResponse
    {
        public int RequisicionItemId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public bool EnUnidades { get; set; }
        public string Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public decimal? PrecioSgm { get; set; }
        public bool PrecioIsValid { get; set; }
        public int? FactorConversion { get; set; }
    }
}
