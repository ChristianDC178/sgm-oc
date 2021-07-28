using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Security
{
    public class UserManager
    {

        public ClaimsIdentity Identity
        {
            get
            {
                return ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            }
        }

        public UserLogged GetLoggedUser(ClaimsPrincipal identity)
        {

            var claims = identity.Claims.Select(x => new { type = x.Type, value = x.Value });

            string userName = null; int sucursalId = 0; List<string> roles = new List<string>();

            foreach (var item in claims)
            {
                switch (item.type)
                {
                    case SgmClaimTypes.SUCURSAL_ID:
                        sucursalId = int.Parse(item.value); break;
                    case SgmClaimTypes.LOGIN_NAME:
                        userName = item.value; break;
                    case SgmClaimTypes.ROL:
                        roles.Add(item.value); break;
                }
            }

            return new UserLogged(userName, roles, sucursalId);
        }

        public string Generate(IEnumerable<Claim> claims)
        {

            //var key = Encoding.ASCII.GetBytes("ANaGHMfwVRnTgJahHduitsHpMKzTFFhtytcZSHkUQFA=");
            var key = Encoding.ASCII.GetBytes("DiarcoDiarcoToken");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity("JWT"),
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Diarco",
                Audience = "Diarco"
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);

        }

    }

}

