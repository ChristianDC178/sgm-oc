using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class UsuarioRol : EntityBase
    {
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }

    }
}





