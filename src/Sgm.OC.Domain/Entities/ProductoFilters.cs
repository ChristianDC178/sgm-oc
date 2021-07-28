using System;

namespace Sgm.OC.Domain.Entities
{
    public class ProductoFilters : FilterBase
    {
        public string IdInterno { get; set; }
        public DateTime? Creacion { get; set; }
        public string Rubro { get; set; }
        public string Recurrente { get; set; }
    }

    public class FilterBase
    {
        public string Id { get; set; }
        public string Page { get; set; }
        public string PageSize { get; set; }
        public string Descripcion { get; set; }
    }

}
    