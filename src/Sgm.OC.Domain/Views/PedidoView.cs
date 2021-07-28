using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Domain.Views
{
    public class PedidoView
    {
        public int PedidoId { get; set; }
        public string PrefijoId
        {
            get
            {
                return "SOL-" + PedidoId;
            }
        }
        public DateTime Creacion { get; set; }
        public DateTime? Modificacion { get; set; }
        public string Descripcion { get; set; }
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

    }
}
