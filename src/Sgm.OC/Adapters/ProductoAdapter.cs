using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;


namespace Sgm.OC.Adapters
{
    public class ProductoAdapter
    {

        public ProductoView Adapt(Producto producto)
        {
            return new ProductoView
            {
                IdInterno = producto.IdInterno,
                UnidadMedida = new UnidadMedidaView()
                {
                    Simbolo = producto.UnidadMedida.Simbolo,
                    Descripcion = producto.UnidadMedida.Descripcion
                },
                Descripcion = producto.Descripcion, 
                FactorConversion = producto.FactorConversion,
                Recurrente = producto.Recurrente
            };
        }

        public List<ProductoView> Adapt (List<Producto> productos)
        {
            List<ProductoView> resultado = new List<ProductoView>();

            foreach(var prod in productos)
            {
                resultado.Add(Adapt(prod));
            }

            return resultado;
        }



    }
}
