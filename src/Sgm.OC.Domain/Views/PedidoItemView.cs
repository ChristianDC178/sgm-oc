using Microsoft.VisualBasic;
using System;

namespace Sgm.OC.Domain.Views
{
    public class PedidoItemView
    {
        public int ProductoId { get; set; }
        public int ProductoIdInterno { get; set; }
        public string Producto { get; set; }
        public int PedidoItemId { get; set; }
        public int Cantidad { get; set; }
        public bool EnUnidades { get; set; }
        public string Sucursal { get; set; }
        //public DateAndTime Creacion { get; set; }
        public string Usuario { get; set; }
        public int RubroIdInterno { get; set; }
        public string Rubro { get; set; }
        public string? RequisicionId { get; set; }
        public string? RequisicionEstadoId { get; set; }
        public DateTime? CreacionRequisicion { get; set; } 
        public string CreacionRequisicionFormatted
        {
            get
            {
                if (CreacionRequisicion.HasValue)
                {
                    return CreacionRequisicion.Value.ToString("dd/MM/yyyy HH:mm");
                } else
                {
                    return "";
                }
            }
        }

        public bool Recurrente { get; set; }
        public bool RequisicionAsignada { get; set; }
        
        public int FactorConversion { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int TotalRows { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int TotalPages { get; set; }
    }
}
