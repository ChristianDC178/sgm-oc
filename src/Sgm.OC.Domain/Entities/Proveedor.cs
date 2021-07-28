using System.ComponentModel.DataAnnotations;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Proveedor : EntityBussiness
    {
        public int IdInterno { get; set; }
        public string Email { get; set; }
    }

}