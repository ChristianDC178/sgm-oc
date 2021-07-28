using System;
using Newtonsoft.Json;

namespace Sgm.OC.Security.Entities
{

    public class UserAuthInfo
    {

        public string Login { get; set; }
        public long Expiration { get; set; }
        public long ExpirationDate { get; set; }
        public int SucursalId { get; set; }

        [JsonIgnore]
        public DateTime Date
        {
            get { return DateTime.FromBinary(ExpirationDate).AddMinutes(Expiration); }
        }

    }

}
