using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Jobs.EmisionOrdenCompra.Entities
{
    public class OCDetalle
    {
        public int Oc { get; set; } = 10;
        public int Prefijo { get; set; }
        public int Sufijo { get; set; }
        public int IdInterno { get; set; }
        public int CantBultosSugerido { get; set; } = 0;
        public int CantBultosProveedor { get; set; }
        public int CantFactorProveedor { get; set; } = 0;
        public int CantBultosBonificados { get; set; } = 0;
        public int CantBultosEmpr { get; set; }
        public int CantFactorEmpr { get; set; }
        public decimal PesoUnit { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal PesoTotalBonif { get; set; } = 0;
        public decimal IvaCalculo { get; set; }
        public decimal CoeficienteIva { get; set; }
        public decimal KImpuestoInterno { get; set; }
        public decimal CostoBase { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioParte { get; set; } = 0;
        public decimal PrecioLista { get; set; } = 0;
        public decimal ImpuestoInterno { get; set; }
        public decimal Envases { get; set; } = 0;
        public decimal TotalImpuestoInterno { get; set; }
        public decimal TotalItem { get; set; }
        public int UnidadesCumplidas { get; set; } = 0;
        public decimal PesoCumplido { get; set; } = 0;
        public char CumplidoParcial { get; set; } = 'N';
        public string UsuarioCumplidoParcial { get; set; } = "0";
        public DateTime FechaCumplidoParcial { get; set; } = new DateTime(1900, 1, 1);
        public decimal PisoPaletizado { get; set; } = 0;
        public decimal AlturaPisoPaletizado { get; set; } = 0;
        public decimal CodigoDescuentoComp1 { get; set; }
        public decimal DescuentoComp1 { get; set; }
        public decimal CodigoDescuentoComp2 { get; set; }
        public decimal DescuentoComp2 { get; set; }
        public decimal CodigoDescuentoComp3 { get; set; }
        public decimal DescuentoComp3 { get; set; }
        public decimal CodigoDescuentoComp4 { get; set; }
        public decimal DescuentoComp4 { get; set; }
        public decimal CodigoDescuentoComp5 { get; set; }
        public decimal DescuentoComp5 { get; set; }
        public decimal CodigoDescuentoComp6 { get; set; }
        public decimal DescuentoComp6 { get; set; }
        public decimal CodigoDescuentoComp7 { get; set; }
        public decimal DescuentoComp7 { get; set; }
        public decimal CodigoDescuentoComp8 { get; set; }
        public decimal DescuentoComp8 { get; set; }
        public decimal CodigoDescuentoComp9 { get; set; }
        public decimal DescuentoComp9 { get; set; }
        public decimal CodigoDescuentoComp10 { get; set; }
        public decimal DescuentoComp10 { get; set; }
    }
}
