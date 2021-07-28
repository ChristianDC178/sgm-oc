using System;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.WF
{
    //Esta es la clase donde vamos a laburar
    public class WorkflowEstado : EntityBase
    {

        public WorkflowEstado() { }

        public int WorkflowId { get; set; }
        public int EstadoId { get; set; }
        public string CodigoRol { get; set; }
        public string NombreEscenario { get; set; }
        public int? Secuencia { get; set; }
        public bool Inicial { get; set; }
        public bool Final { get; set; }
        public bool ComentarioRequerido { get; set; }
        public bool ConfirmacionExterna { get; set; }
        public Estado Estado { get; set; }
        public bool Rechazar { get; set; }

    }

}
