using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Sgm.OC.Security.Entities
{

    public class LoginResult
    {
        public string LoginName { get; set; }
        public bool IsOk { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string AuthToken { get; set; }
        public DateTime LoginDate { get; set; }
        public string FullName { get; set; }
        public string Sucursal { get; set; }
    }

}
