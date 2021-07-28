using Sgm.OC.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Domain.Entities
{
    public class Rubro : EntityBase
    {
        public  string Id { get; set; }
        public string Descripcion { get; set; }
        public int IdInterno { get; set; }
        public int IdPadreInterno { get; set; }

    }
}
