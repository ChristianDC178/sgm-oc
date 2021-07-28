using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sgm.OC.Adapters;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain.Entities;
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
    [Route("api/pedidos")]
    public class PedidoController : SgmController
    {

        ILogger<PedidoController> _logger;
        PedidoBussiness _pedidoBussiness;
        UserManager _userManager = new UserManager();

        public PedidoController(ILogger<PedidoController> logger)
        {
            _logger = logger;

            _pedidoBussiness = new PedidoBussiness();
        }

        [HttpPost]
        public IActionResult CreatePedido(PedidoRequest model)
        {
            return ExecuteResponse(() =>
            {

                //TODO because: https://docs.microsoft.com/en-us/aspnet/core/migration/claimsprincipal-current?view=aspnetcore-3.1
                Sgm.OC.Security.Entities.UserLogged UserLogged = _userManager.GetLoggedUser(this.User);

                List<PedidoItem> items = new PedidoAdapter().AdaptRequest(model);
                _pedidoBussiness.UserLogged = UserLogged;
                BusinessResult<Pedido> result = _pedidoBussiness.CreatePedido(model.Recurrente, items);

                ResponseApi responseApi = new ResponseApi();

                if (result.Validation.IsValid)
                {
                    responseApi.StatusCode = HttpStatus.Status201Created;
                    responseApi.Data = result.Data.Id;
                }
                else
                {
                    responseApi.Validations = result.Validation.GetMessages();
                    responseApi.StatusCode = HttpStatus.Status400BadRequest;
                }

                return responseApi;

            });
        }

        [HttpPut]
        [Route("{pedidoId}")]
        public IActionResult UpdatePedido(int pedidoId, List<PedidoItemRequest> items)
        {
            return ExecuteResponse(() =>
            {
                PedidoBussiness pedidoBussiness = new PedidoBussiness();
                Sgm.OC.Security.Entities.UserLogged UserLogged = _userManager.GetLoggedUser(this.User);

                ResponseApi responseApi = new ResponseApi();
                _pedidoBussiness.UserLogged = UserLogged;

                PedidoAdapter adapter = new PedidoAdapter();

                BusinessResult<bool> result = _pedidoBussiness.UpdatePedido(pedidoId, adapter.AdaptPedidoItemRequest(items));

                if (result.Validation.IsValid)
                {
                    responseApi.StatusCode = HttpStatus.Status202Accepted;
                    responseApi.Data = result.Data;
                }
                else
                {
                    responseApi.Validations = result.Validation.GetMessages();
                    responseApi.StatusCode = HttpStatus.Status400BadRequest;
                }

                return responseApi;

            });
        }

        [HttpPut]
        [Route("{pedidoId}/estados")]
        public IActionResult SetNewEstado(ChangeEstadoRequest request, int pedidoId)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();

                UserLogged UserLogged = _userManager.GetLoggedUser(this.User);
                PedidoBussiness pedidoBussiness = new PedidoBussiness();
                pedidoBussiness.UserLogged = UserLogged;

                WorkflowEstadoEntidadAdapter adapter = new WorkflowEstadoEntidadAdapter();
                BusinessResult<ChangeWorkflowEstadoResult> businessResult = pedidoBussiness.ChangeEstado(pedidoId, request.Comentario, request.Rechazar);

                if (businessResult.Validation.IsValid)
                {
                    responseApi.Data = adapter.Adapt(businessResult.Data.WorkflowEstados);
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = businessResult.Validation.GetMessages();
                }

                return responseApi;
            });
        }

    }
}


