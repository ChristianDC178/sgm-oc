using System.Data.Common;
using System.Text;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Producto : EntityBussiness
    {

        public Producto() { }

        public string Descripcion { get; set; }

        ///Sirve para 
        public int IdInterno { get; set; }
        //public string RubroId { get; set; }
        public Rubro Rubro { get; set; }
        //public int  IdFamilia { get; set; }
        public decimal? Precio { get; set; }
        public int FactorConversion { get; set; } = 1;

        public bool Recurrente { get; set; }

        private UnidadMedida _unidadMedida;

        public int UnidadMedidaId
        {
            get; private set;
        }

        public UnidadMedida UnidadMedida
        {

            get
            {
                return _unidadMedida;
            }
            set
            {
                UnidadMedidaId = value.Id;
                _unidadMedida = value;
            }
        }


    }



}





