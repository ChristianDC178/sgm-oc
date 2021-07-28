using Sgm.OC.Domain;
using Sgm.OC.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgm.OC.Adapters
{
    public class EstadoAdapter
    {
        public List<EstadoView> AdaptToView(List<Estado> estados)
        {
            List<EstadoView> result = new List<EstadoView>();

            foreach (var item in estados)
            {
                result.Add(AdaptToView(item));
            }

            return result;
        }

        public EstadoView AdaptToView(Estado estado)
        {
            return new EstadoView
            {
                Descripcion = estado.Descripcion,
                Id = estado.Id
            };
        }

    }
}
