using System.Collections.Generic;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.WF
{
    //Estas 2 clases son un template
    public class Workflow : EntityBussiness
    {
        public Workflow() { }

        public List<WorkflowEstado> Estados { get; set; } = new List<WorkflowEstado>();

        public List<WorkflowEstadoEntidad> GetWorkflowToEntity()
        {
            List<WorkflowEstadoEntidad> workflowEstadoEntidads = new List<WorkflowEstadoEntidad>();

            Estados.ForEach((est) =>
            {
                var estadoEntidad = new WorkflowEstadoEntidad()
                {
                    EstadoId = est.EstadoId,
                    CodigoRol = est.CodigoRol,
                    Secuencia = est.Secuencia,
                    Inicial = est.Inicial,
                    Final = est.Final,
                    ComentarioRequerido = est.ComentarioRequerido,
                    ConfirmacionExterna = est.ConfirmacionExterna, 
                    NombreEscenario = est.NombreEscenario, 
                    Rechazar = est.Rechazar
                };

                workflowEstadoEntidads.Add(estadoEntidad);
            });

            return workflowEstadoEntidads;

        }

    }

    public class WorkflowTypeConstants
    {
        public const int PRE_VALIDACION = 1;
        public const int VALIDACION = 2;
        public const int REQUISICION  = 3;
    }
}
