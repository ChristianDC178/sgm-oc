using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Sgm.OC.Adapters
{
    public class SucursalAdapter
    {
        public List<SucursalView> AdaptToView(List<Sucursal> sucursales)
        {
            List<SucursalView> result = new List<SucursalView>();

            foreach (var item in sucursales)
            {
                result.Add(AdaptToView(item));
            }

            return result;

        }

        public SucursalView AdaptToView(Sucursal sucursal)
        {
            return new MapperConfiguration(cfg =>
           {
               cfg.CreateMap<Sucursal, SucursalView>()
                    .ForMember(d => d.Descripcion, o => o.MapFrom(o => o.Descripcion))
                    .ForMember(d=>d.Id,o=>o.MapFrom(o=>o.Id));

           }).CreateMapper().Map<Sucursal, SucursalView>(sucursal);
        }
    }
}
