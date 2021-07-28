namespace Sgm.OC.Models.Responses
{
    public class WorkflowEstadoEntidadResponse
    {

        public bool? Aprobado { get; set; }
        public string Comentario { get; set; }
        public string CodigoRol { get; set; }
        public int? Secuencia { get; set; }
        public bool Inicial { get; set; }
        public bool Final { get; set; }
        public bool ComentarioRequerido { get; set; }
        public bool IsCurrent { get; set; }
        public bool Rechazar { get; set; }
        public string NombreEscenario { get; set; }
        public EstadoResponse Estado { get; set; }
    }

    public class EstadoResponse
    {
        public int EstadoId { get; set; }
        public string Descripcion { get; set; }

        public EstadoResponse(int estadoId, string descripcion)
        {
            EstadoId = estadoId;
            Descripcion = descripcion;
        }
    }

}
