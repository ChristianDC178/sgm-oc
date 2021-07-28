using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sgm.OC.Adapters;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Views;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Models.Responses;
using Sgm.OC.Models.Views;
using Sgm.OC.Security;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Controllers
{

    [ApiController]
    [Route("api/pedidos")]
    public class PedidoQueryController : SgmController
    {

        ILogger<PedidoController> _logger;
        PedidoBussiness _pedidoBussiness;
        UserManager _userManager = new UserManager();

        public PedidoQueryController(ILogger<PedidoController> logger)
        {
            _logger = logger;

            _pedidoBussiness = new PedidoBussiness();
        }

        [HttpGet]
        [Route("{pedidoId}")]
        public IActionResult GetById(int pedidoId)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                PedidoAdapter pedidoAdapter = new PedidoAdapter();
                Pedido pedido = _pedidoBussiness.GetById(pedidoId);

                if (pedido != null && pedido.SucursalId != 0)
                {
                    responseApi.Data = pedidoAdapter.AdaptToResponse(pedido);
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    _logger.LogInformation("No se encontro ningun pedido");
                    responseApi.StatusCode = StatusCodes.Status404NotFound;
                }

                return responseApi;

            });
        }

        [HttpGet]
        public IActionResult GetPedidoByFilters([FromQuery] PedidoFilters filtro)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                PedidoAdapter pedidoAdapter = new PedidoAdapter();

                UserLogged UserLogged = _userManager.GetLoggedUser(this.User);
                _pedidoBussiness.UserLogged = UserLogged;
                

                ViewResult<Sgm.OC.Domain.Views.PedidoView> pedidos = _pedidoBussiness.GetPedidoByFilter(filtro);
                if (pedidos.Validation.IsValid)
                {
                    responseApi.Data = pedidos;
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = pedidos.Validation.GetMessages();
                }
                return responseApi;
            });
        }

        [HttpGet]
        [Route("pedidos_items")]
        public IActionResult GetPedidosItemsByFilters([FromQuery] Domain.Filters.PedidoItemViewFilters filter)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                PedidoBussiness pedidoBussiness = new PedidoBussiness();
                responseApi.StatusCode = StatusCodes.Status200OK;
                responseApi.Data = pedidoBussiness.GetPedidoItemsByFilter(filter);
                return responseApi;
            });
        }

        [HttpGet]
        [Route("{pedidoId}/estados")]
        public IActionResult GetWorkflow(int pedidoId)
        {
            return ExecuteResponse(() =>
            {

                ResponseApi responseApi = new ResponseApi();
                PedidoBussiness pedidoBussiness = new PedidoBussiness();
                BusinessResult<List<WorkflowEstadoEntidad>> businessResult = pedidoBussiness.GetPedidoWorkflow(pedidoId);

                if (businessResult.Validation.IsValid)
                {
                    List<WorkflowEstadoEntidadResponse> model = new WorkflowEstadoEntidadAdapter().Adapt(businessResult.Data);
                    responseApi.StatusCode = StatusCodes.Status200OK;
                    responseApi.Data = model;
                }
                else
                {
                    responseApi.Validations = businessResult.Validation.GetMessages();
                    responseApi.StatusCode = StatusCodes.Status404NotFound;
                }

                return responseApi;
            });
        }

    }
}
