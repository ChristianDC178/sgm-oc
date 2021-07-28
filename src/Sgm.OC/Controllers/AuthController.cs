using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgm.OC.Core.Bussiness;

namespace Sgm.OC.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AuthController : SgmController
    {

        private readonly ILogger<AuthController> _logger;
        UsuarioBusiness _UsuarioBussiness = new UsuarioBusiness();

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Authenticate([FromQuery] string token)
        {
            return ExecuteResponse(() =>
            {

                Sgm.OC.Security.Entities.LoginResult LoginResult = _UsuarioBussiness.Login(token);

                ResponseApi response = new ResponseApi();

                if (LoginResult.IsOk)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Data = LoginResult;
                    return response;
                }

                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            });

        }

    }

}
