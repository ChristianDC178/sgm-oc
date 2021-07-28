using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgm.OC.Adapters
{
    public class ProveedorAdapter
    {
        public List<ProveedorView1> AdaptToView(List<Proveedor> proveedores)
        {
            List<ProveedorView1> result = new List<ProveedorView1>();

            foreach (var item in proveedores)
            {
                result.Add(AdaptToView(item));
            }

            return result;
        }

        public ProveedorView1 AdaptToView(Proveedor proveedor)
        {
            return new ProveedorView1
            {
                Descripcion = proveedor.Descripcion,
                Email = proveedor.Email,
                IdInterno = proveedor.IdInterno.ToString()
                
            };
        }
    }
}
