using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sgm.OC.Security.Entities;
using Sgm.OC.Security;

namespace Sgm.OC.Controllers
{
    public class SgmController : ControllerBase
    {

        UserManager _userManager = new UserManager();

        [NonAction]
        public IActionResult ExecuteResponse(Func<ResponseApi> func)
        {
            try
            {
                ResponseApi responseApi = func.Invoke();
                return StatusCode(responseApi.StatusCode, responseApi);
            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Fatal("Error inesperado", ex);
                return CreateBadRequestResponse();
            }
        }

        private IActionResult CreateBadRequestResponse()
        {
            var responseApi = new ResponseApi()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Validations = new List<string>() { "Ocurrio un problema inesperado" }
            };

            return StatusCode(responseApi.StatusCode, responseApi);
        }


        public UserLogged UserLogged
        {
            get
            {

                if (this.User.Claims.Count() == 0)
                    return null;

                return _userManager.GetLoggedUser(this.User);
            }
        }

    }
}
