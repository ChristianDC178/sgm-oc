using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Framework.Encription;
using Sgm.OC.Repositories;
using Sgm.OC.Repository;
using Sgm.OC.Repository.Extensions;
using Sgm.OC.Security;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Core.Bussiness
{
    public class UsuarioBusiness
    {

        private class UserInfoTemp
        {
            public string Login { get; set; }
            public int Sucursal { get; set; }
        }


        public LoginResult Login(string encryptedtoken)
        {

            LoginResult result = new LoginResult();

            try
            {

                //Workaround
                UserInfoTemp userAuthInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoTemp>(encryptedtoken);
                return Login(userAuthInfo.Login, userAuthInfo.Sucursal);

                //string desencriptado = EncryptedPasswordManager.ForPassword("Diarco").Decrypt(encryptedtoken);
                //UserAuthInfo userinfo = JsonConvert.DeserializeObject<UserAuthInfo>(desencriptado);
                //
                //if (userinfo.Date <= DateTime.Now)
                //{
                //    return result;
                //}
                //
                //result = Login(userinfo.Login, userinfo.SucursalId);
                //
                //return result;

            }
            catch (Exception ex)
            {
                return result;
            }

        }

        public LoginResult Login(string userName, int sucursalId)
        {

            LoginResult result = new LoginResult();

            UnitOfWork UserUoW = new UnitOfWork(new SgmOcContext());

            Usuario usuario = UserUoW.UsuarioRepo.LoadUsuario(userName, sucursalId);

            if (usuario == null || usuario?.Roles.Count == 0)
            {
                return result;
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(SgmClaimTypes.LOGIN_NAME, usuario.Login));
            claims.Add(new Claim(SgmClaimTypes.SUCURSAL_ID, usuario.SucursalId.ToString()));
            foreach (UsuarioRol rol in usuario.Roles)
            {
                claims.Add(new Claim(SgmClaimTypes.ROL, rol.Rol.Codigo));
            }

            string token = new UserManager().Generate(claims);

            result.AuthToken = token;
            result.IsOk = true;
            result.LoginDate = DateTime.Now;
            result.LoginName = usuario.Login;
            result.FullName = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido);
            result.Sucursal = usuario.Sucursal.Descripcion;
            result.Roles.AddRange(usuario.Roles.Select(rol => rol.Rol.Codigo));

            return result;
        }

    }
}
