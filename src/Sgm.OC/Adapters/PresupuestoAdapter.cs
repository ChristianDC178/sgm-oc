using System.Collections.Generic;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Responses;

namespace Sgm.OC.Adapters
{
    public class PresupuestoAdapter
    {
        public PresupuestoResponse Adapt(Presupuesto presupuesto)
        {
            return new PresupuestoResponse()
            {
                PresupuestoId = presupuesto.Id,
                Aprobado = presupuesto.Aprobado,
                Cotizacion = presupuesto.Cotizacion,
                TieneArchivo = presupuesto.Archivo != null,
                ComentarioAprobacion = presupuesto.ComentarioAprobacion,
                Proveedor = new ProveedorResponse()
                {
                    IdInterno = presupuesto.Proveedor.IdInterno,
                    Descripcion = presupuesto.Proveedor.Descripcion,
                    Email = presupuesto.Proveedor.Email,
                }
            };
        }

        public List<PresupuestoResponse> Adapt(List<Presupuesto> presupuestos)
        {

            List<PresupuestoResponse> presupuestoResponses = new List<PresupuestoResponse>();

            presupuestos.ForEach((p) =>
            {
                presupuestoResponses.Add(Adapt(p));
            });

            return presupuestoResponses;
        }

    }

}
