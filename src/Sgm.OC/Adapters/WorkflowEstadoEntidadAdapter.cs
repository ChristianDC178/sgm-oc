using System.Collections.Generic;
using Sgm.OC.Domain.WF;
using Sgm.OC.Models.Responses;

namespace Sgm.OC.Adapters
{
    public class WorkflowEstadoEntidadAdapter
    {

        public WorkflowEstadoEntidadResponse Adapt(WorkflowEstadoEntidad source)
        {
            return new WorkflowEstadoEntidadResponse()
            {
                Aprobado = source.Aprobado,
                Comentario = source.Comentario,
                CodigoRol = source.CodigoRol,
                ComentarioRequerido = source.ComentarioRequerido,
                Estado = new EstadoResponse(source.Estado.Id, source.Estado.Descripcion),
                Inicial = source.Inicial,
                IsCurrent = source.IsCurrent,
                Rechazar = source.Rechazar,
                Final = source.Final,
                Secuencia = source.Secuencia, 
                NombreEscenario = source.NombreEscenario
            };
        }
            
        public List<WorkflowEstadoEntidadResponse> Adapt(List<WorkflowEstadoEntidad> lsWorkflowEstadoEntidad)
        {
            List<WorkflowEstadoEntidadResponse> lsModels = new List<WorkflowEstadoEntidadResponse>();
            lsWorkflowEstadoEntidad.ForEach(we =>
            {
                lsModels.Add(this.Adapt(we));
            });

            return lsModels;
        }

    }
}
