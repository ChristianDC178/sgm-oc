namespace Sgm.OC.Models.Responses
{
    public class PresupuestoResponse
    {
        public int PresupuestoId { get; set; }
        public bool? Aprobado { get; set; }
        public decimal? Cotizacion { get; set; }
        public bool TieneArchivo { get; set; }
        public string ComentarioAprobacion { get; set; }
        public ProveedorResponse Proveedor { get; set; }
    }

    public class ProveedorResponse
    {
        public int IdInterno { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
    }
}
