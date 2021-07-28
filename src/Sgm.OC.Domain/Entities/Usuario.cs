using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Microsoft.AspNetCore.Components.Forms;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Usuario : EntityBase
    {

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int SucursalId { get; set; }
        public string Login { get; set; }

        public Sucursal Sucursal { get; set; }
        public Pedido Pedido { get; set; }
        public List<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();

        public bool IsInRole(string codigoRol)
        {

            if (!Roles.Any())
                return false;

            return Roles.Select(rol => rol.Rol.Codigo).Contains(codigoRol);
        }

        public bool IsInRole(params string[] codigoRoles)
        {
            if (!Roles.Any())
                return false;

            List<string> roles = Roles.Select(r => r.Rol.Codigo).ToList();

            foreach (string codRol in codigoRoles)
            {
                if (roles.Contains(codRol))
                    return true;
            }

            return false;
        }

        public bool CanApproveByMonto(decimal monto)
        {
            return Roles.Select(userRol => userRol.Rol).Where(r => monto <= r.MontoAprobacion).Count() > 0;
        }

        public decimal GetMontoMaximo()
        {
            var monto =  Roles.Select(userRol => userRol.Rol).Max(rol => rol.MontoAprobacion);
            return monto.HasValue ? monto.Value : 0;
        }

    }

}





