using System;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Presupuesto : EntityBussiness
    {

        public int RequisicionId { get; set; }
        public int UsuarioId { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public bool Aprobado { get; set; }
        public decimal? Cotizacion { get; set; }
        public int ProveedorId { get; set; }
        public string Email { get; set; }
        public string Comentario { get; set; }
        public string ComentarioAprobacion { get; set; }

        public Proveedor Proveedor { get; set; }
        public Usuario Usuario { get; set; }
        public Archivo Archivo { get; set; }
    }

    public class Archivo
    {
        public Guid? ArchivoId { get; set; }
        public string Nombre { get; set; }
        public string Path { get; set; }
    }

}
