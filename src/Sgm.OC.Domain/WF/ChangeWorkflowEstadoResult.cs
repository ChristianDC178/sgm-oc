using System.Collections.Generic;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.WF
{
    public class ChangeWorkflowEstadoResult
    {
        public WorkflowEstadoEntidad EstadoAprobado { get; set; }
        public WorkflowEstadoEntidad EstadoSiguiente { get; set; }
        public List<WorkflowEstadoEntidad> WorkflowEstados { get; set; } = new List<WorkflowEstadoEntidad>();
        public Validation Validation { get; set; } = new Validation();
        public bool HasNext
        {
            get { return EstadoSiguiente != null; }
        }

    }
}
