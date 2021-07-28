using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Jobs.Precios.Entities
{
    public class ItemsSGM
    {
        public int IdInterno { get;set; }
        public decimal Precio { get; set; }
        public int Factor { get; set; }
    }
}
