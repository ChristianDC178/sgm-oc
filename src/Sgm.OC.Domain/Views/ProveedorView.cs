using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Domain.Views
{
    public class ProveedorView
    {
        public string Email { get; set; }
        public int IdInterno { get; set; }
        public string Descripcion { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int TotalRows { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int TotalPages { get; set; }
    }
}
