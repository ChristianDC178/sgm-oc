using System.Collections.Generic;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Sucursal : EntityBase
    {
        public string Descripcion { get; set; }
        public bool CasaCentral { get; set; }
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public bool? PorDefectoAEntregar { get; set; }
    }

}