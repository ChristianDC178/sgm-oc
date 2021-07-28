using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgm.OC.Adapters
{
    public class RubroAdapter
    {
        public List<RubroView> AdaptToView(List<Rubro> rubros)
        {
            List<RubroView> result = new List<RubroView>();

            foreach(var item in rubros)
            {
                result.Add(AdaptToView(item));
            }
            return result;
        }

        public RubroView AdaptToView(Rubro rubro)
        {
            return new RubroView
            {
                Descripcion = rubro.Descripcion,
                Id = rubro.Id.ToString(),
                IdInterno = rubro.IdInterno
            };
        }

    }
}
