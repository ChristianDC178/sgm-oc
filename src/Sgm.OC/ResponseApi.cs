using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace Sgm.OC
{
    public class ResponseApi
    {

        public object Data { get; set; }
        public int StatusCode { get; set; }

        public bool IsValid
        {
            get
            {
                return StatusCode >= 200 && StatusCode <= 400;
            }
        }

        public List<string> Validations { get; set; } = new List<string>();

    }
}
