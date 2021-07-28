using System;
using Sgm.OC.Domain.ValueOjects;

namespace Sgm.OC.Domain.Views
{
    public class RequisicionView
    {

        public int RequisicionId { get; set; }

        public string PrefijoId
        {
            get
            {
                return "REQ-" + RequisicionId;
            }
        }

        public DateTime Creacion { get; set; }
        public DateTime? Modificacion { get; set; }
        public string Descripcion{ get; set; }
        public int SucursalId { get; set; }
        public bool Recurrente { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string? UsuarioModificacion { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int TotalRows { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int TotalPages { get; set; }

        public string CreacionFormated
        {
            get
            {
                return Creacion.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string ModificacionFormated
        {
            get
            {
                if (Modificacion.HasValue)
                {

                    return Modificacion.Value.ToString("dd/MM/yyyy HH:mm");
                }
                return null;
            }
        }
        
        public int? Codigo { get; set; }
        public int? Sufijo { get; set; }
        public int? Prefijo { get; set; }
        public string Tipo { get; set; }

        public string NroOC
        {
            get
            {
                if (Codigo.HasValue && Sufijo.HasValue && Prefijo.HasValue)
                {
                    return $"{Codigo.Value}-{Sufijo.Value}-{Prefijo.Value}";
                }

                return string.Empty;
            }
        }

    }
}
