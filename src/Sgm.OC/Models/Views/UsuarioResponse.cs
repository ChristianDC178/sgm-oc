using Sgm.OC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sgm.OC.Models.Views
{
    public class UsuarioResponse
    {
        public string Nombre { get; set; }
        public string Sucursal { get; set; }
        public string Login { get; set; }
        public List<RolResponse> Roles { get; set; }
        public string AppVersion { get; set; }
      
    }

    public class RolResponse
    {
        public string Descripcion { get; set; }
        public decimal? MontoAprobacion { get; set; } 
    }
}
