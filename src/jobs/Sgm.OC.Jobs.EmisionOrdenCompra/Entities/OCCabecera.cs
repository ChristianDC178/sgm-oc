using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Jobs.EmisionOrdenCompra.Entities
{
    public class OCCabecera
    {
        public int Oc { get; set; } = 10;
        public int Prefijo { get; set; } = 1;
        public int Sufijo { get; set; }
        public int PrefijoLote { get; set; } = 1;
        public int Lote { get; set; }
        public char OcMadre { get; set; } = 'S';
        public char OcTransferencia { get; set; } = 'N';
        public char OcPagoAnterior { get; set; } = 'N';
        public int DiasLimiteEntrega { get; set; }
        public int IdProveedor { get; set; }
        public int IdSucursal { get; set; } = 1;
        public int IdSucursalDestino { get; set; } = 0;
        public int IdSucursalDestinoAlt { get; set; } = 0;
        public int Situacion { get; set; } = 1;
        public DateTime FechaSituacion { get; set; } //getdate
        public DateTime FechaAlta { get; set; } //getdate
        public DateTime FechaEmision { get; set; } //getdate
        public DateTime FechaEntrega { get; set; } = new DateTime(1900,1,1);
        public decimal ImporteNeto { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal ImporteImpuestoInt { get; set; }
        public decimal ImporteTotal { get; set; }
        public int CompradorId { get; set; }
        public int OperadorId { get; set; }
        public string TerminalOperador { get; set; }
        public string UsuarioCumplido { get; set; }
        public string Tipo { get; set; } = "F";
        public int PlazoEntrega1 { get; set; }
        public int PlazoEntrega2 { get; set; } = 0;
        public int PlazoEntrega3 { get; set; } = 0;
        public int PlazoEntrega4 { get; set; } = 0;
        public int PlazoEntrega5 { get; set; } = 0;
        public int PlazoEntrega6 { get; set; } = 0;
        public int CondicionPago { get; set; }
        public string Observacion { get; set; }
        public DateTime FechaIng { get; set; } = new DateTime(1900, 1, 1);
        public int CodigoIng { get; set; } = 0;
        public int PrefijoCodigoIng { get; set; } = 0;
        public int SufijoCodigoIng { get; set; } = 0;
        public string ObservacionIng { get; set; } = " ";
        public int TipoEntrega { get; set; } = 0;
        public string UsuarioModifId { get; set; }
        public string TerminalModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public char OcElectronica { get; set; } = 'N';
        public int SituacOc { get; set; } = 0;
        public DateTime FechaSituacOc { get; set; } = new DateTime(1900, 1, 1);
        public char Enviado { get; set; } = 'N';
        //public char Especial { get; set; } = 'N';
        //public int TipoProveedor { get; set; }
        public List<OCDetalle> Detalles { get; set; } = new List<OCDetalle>();

    }
}
