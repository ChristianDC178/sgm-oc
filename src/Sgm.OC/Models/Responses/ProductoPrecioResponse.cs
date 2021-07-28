using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgm.OC.Models.Responses
{
    public class ProductoPrecioResponse
    {
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public int PrecioSGM { get; set; }
    }
}
