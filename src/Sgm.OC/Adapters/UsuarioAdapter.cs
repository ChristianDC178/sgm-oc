using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sgm.OC.Adapters
{
    public class UsuarioAdapter
    {
        public UsuarioResponse AdaptToView(Usuario usuario)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return new UsuarioResponse
            {
                Nombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido),
                Login = usuario.Login,
                Roles = AdaptRoles(usuario.Roles),
                Sucursal = usuario.Sucursal.Descripcion,
                AppVersion = assembly.GetName().Version.ToString()

            };
        }

        public List<RolResponse> AdaptRoles(List<UsuarioRol> roles)
        {
            List<RolResponse> respuesta = new List<RolResponse>();
            foreach(var rol in roles)
            {
                RolResponse UsuarioRol = new RolResponse();
                UsuarioRol.Descripcion = rol.Rol.Descripcion;
                UsuarioRol.MontoAprobacion = rol.Rol.MontoAprobacion;
                respuesta.Add(UsuarioRol);
            }
            return respuesta;
        }
    }
}
