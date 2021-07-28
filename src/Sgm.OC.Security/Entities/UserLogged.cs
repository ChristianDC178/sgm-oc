using System;
using System.Collections.Generic;
using System.Text;

namespace Sgm.OC.Security.Entities
{
    public class UserLogged
    {

        public UserLogged(string loginName, List<string> roles, int sucursalId)
        {
            if (string.IsNullOrEmpty(loginName) || roles.Count == 0)
                throw new System.Security.SecurityException("The user cannot be null and must have roles");

            Login = loginName;
            Roles = roles;
            SucursalId = sucursalId;
        }

        public int SucursalId { get; set; }
        public string Login { get; private set; }
        public List<string> Roles { get; private set; } = new List<string>();


        public bool IsInRole(params string[] roles)
        {
            foreach (var item in roles)
            {
                if (Roles.Contains(item))
                    return true;
            }

            return false;
        }

    }
}
