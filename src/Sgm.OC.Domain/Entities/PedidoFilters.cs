using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Domain.Entities
{
    public class PedidoFilters : FilterBase
    {
        public string Id { get; set; }
        public string Sucursal { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public string Usuario { get; set; }

        public int? UsuarioId { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }


       
    }
}
