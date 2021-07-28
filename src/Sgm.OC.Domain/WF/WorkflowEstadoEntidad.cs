using System;
using System.Collections.Generic;
using System.Text;
using Sgm.OC.Framework;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Domain.WF
{
    public class WorkflowEstadoEntidad : EntityBase
    {

        public WorkflowEstadoEntidad()
        { 
        
        }

        public int EstadoId { get; set; }
        public bool? Aprobado { get; set; }
        public string Comentario { get; set; }
        public int? UsuarioId { get; set; }
        public string CodigoRol { get; set; }
        public string  NombreEscenario { get; set; }
        public int? Secuencia { get; set; }
        public bool Inicial { get; set; }
        public bool Final { get; set; }
        public bool ComentarioRequerido { get; set; }
        public bool ConfirmacionExterna { get; set; }
        public DateTime? FechaEstado { get; set; }
        public int? RequisicionId { get; set; }
        public int? PedidoId { get; set; }
        public bool IsCurrent { get; set; }
        public bool Rechazar { get; set; }
        public Usuario Usuario { get; set; }
        public Estado Estado { get; set; }
    }
}
