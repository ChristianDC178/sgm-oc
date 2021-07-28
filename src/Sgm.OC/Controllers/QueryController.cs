using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sgm.OC.Adapters;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Enums;
using Sgm.OC.Domain.Views;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Models.Requests;
using Sgm.OC.Models.Responses;
using Sgm.OC.Models.Views;
using Sgm.OC.Security;
using Sgm.OC.Security.Entities;
using HttpStatus = Microsoft.AspNetCore.Http.StatusCodes;

namespace Sgm.OC.Controllers
{
    [ApiController]
    [Route("api")]
    public class QueryController : SgmController
    {
        ILogger<QueryController> _logger;
        QueryBussiness _queryBussiness;
        UserManager _userManager = new UserManager();

        public QueryController(ILogger<QueryController> logger)
        {
            _logger = logger;

            _queryBussiness = new QueryBussiness();

        }

        [HttpGet]
        [Route("estados/{estadoId}")]
        public IActionResult GetByTipoEntidad(TipoEntidad estadoId)
        {
            return ExecuteResponse(() =>
            {
                List<Estado> estados = _queryBussiness.GetByTipoEntidad(estadoId);
                List<EstadoView> result = new EstadoAdapter().AdaptToView(estados);
                ResponseApi responseApi = new ResponseApi()
                {
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };

                return responseApi;
            });

        }

        [HttpGet]
        [Route("sucursales")]
        public IActionResult GetSucursal([FromQuery] SucursalFilters filtro)
        {
            return ExecuteResponse(() =>
            {
                List<Sucursal> sucursales = _queryBussiness.GetBySucursalByFilter(filtro);
                List<SucursalView> result = new SucursalAdapter().AdaptToView(sucursales);
                return new ResponseApi()
                {
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            });
        }

        [HttpGet]
        [Route("rubros")]
        public IActionResult GetRubroByFilter([FromQuery] RubroFilters filtro)
        {
            return ExecuteResponse(() =>
            {
                List<Rubro> rubros = _queryBussiness.GetRubroByFiltro(filtro);
                List<RubroView> result = new RubroAdapter().AdaptToView(rubros);
                return new ResponseApi()
                {
                    Data = result,
                    StatusCode = StatusCodes.Status200OK
                };
            });
        }

        [HttpGet]
        [Route("proveedores")]
        public IActionResult GetProveedorByFilter([FromQuery] ProveedorFilters filtro)
        {
            return ExecuteResponse(() =>
            {
                ViewResult<ProveedorView> proveedores = _queryBussiness.GetProveedorByFilter(filtro);
                //List<ProveedorView> result = new ProveedorAdapter().AdaptToView(proveedores);
                return new ResponseApi()
                {
                    Data = proveedores,
                    StatusCode = StatusCodes.Status200OK
                };
            });
        
        }

        [HttpGet]
        [Route("{usuario}")]
        public IActionResult GetUsuario()
        {
            return ExecuteResponse(() =>
            {
                _queryBussiness.UserLogged = UserLogged;
                Usuario usuario = _queryBussiness.GetUsuario();

                UsuarioResponse res = new UsuarioAdapter().AdaptToView(usuario);

                
                return new ResponseApi()
                {
                      Data = res,
                    StatusCode = StatusCodes.Status200OK
                };
            });
        }
    }
}

