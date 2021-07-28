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
using Sgm.OC.Models.Responses;

namespace Sgm.OC.Controllers
{

    [ApiController]
    [Route("api/requisiciones")]
    public class RequisicionQueryController : SgmController
    {

        RequisicionBussiness _requisicionBussiness = new RequisicionBussiness();
        ILogger<RequisicionController> _logger;

        public RequisicionQueryController(ILogger<RequisicionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{requisicionId}")]
        public IActionResult GetById(int requisicionId)
        {
            return ExecuteResponse(() =>
            {

                ResponseApi responseApi = new ResponseApi();
                RequisicionAdapter requisicionAdapter = new RequisicionAdapter();
                Requisicion requisicion = _requisicionBussiness.GetById(requisicionId);

                if (requisicion != null && requisicion.SucursalId != 0)
                {
                    responseApi.Data = requisicionAdapter.AdaptToResponse(requisicion);
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    _logger.LogInformation("No se encontro ninguna requisicion");
                    responseApi.StatusCode = StatusCodes.Status404NotFound;
                }

                return responseApi;

            });
        }

        [HttpGet]
        [Route("sucursales")]
        public IActionResult GetSucursales()
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();

                responseApi.Data = _requisicionBussiness.GetSucursales();
                responseApi.StatusCode = StatusCodes.Status200OK;

                return responseApi;
            });
        }

        [HttpGet]
        public IActionResult GetRequisicionByFilters([FromQuery] RequisicionFilters filtro)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                RequisicionAdapter requisicionAdapter = new RequisicionAdapter();

                ViewResult<Sgm.OC.Domain.Views.RequisicionView> requisiciones = _requisicionBussiness.GetRequisicionByFilter(filtro);


                if (requisiciones.Validation.IsValid)
                {
                    responseApi.Data = requisiciones;
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = requisiciones.Validation.GetMessages();
                }

                return responseApi;

            });
        }

        [HttpGet]
        [Route("{requisicionId}/estados")]
        public IActionResult GetWorkflow(int requisicionId)
        {
            return ExecuteResponse(() =>
            {

                ResponseApi responseApi = new ResponseApi();
                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                BusinessResult<List<WorkflowEstadoEntidad>> businessResult = requisicionBussiness.GetRequisicionWorkflow(requisicionId);

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


        [HttpGet]
        [Route("{requisicionId}/presupuestos")]
        public IActionResult GetPresupuestos(int requisicionId)
        {
            return ExecuteResponse(() =>
            {
                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                List<Presupuesto> presupuestos = presupuestoBussiness.GetPresupuestosByRequisicion(requisicionId);
                List<PresupuestoResponse> presupuestosResponse = new PresupuestoAdapter().Adapt(presupuestos);

                return new ResponseApi()
                {
                    Data = presupuestosResponse,
                    StatusCode = StatusCodes.Status200OK,
                };
            });
        }

        [HttpGet]
        [Route("{requisicionId}/pedido_items/{productoId}")]
        public IActionResult GetRequisicionItemsDetails(int requisicionId, int productoId)
        {
            return ExecuteResponse(() =>
            {
                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                List<PedidoItem> pedidoItems = requisicionBussiness.GetPedidoItemsByRequisicionId(requisicionId, productoId);
                List<PedidoItemDetailResponse>  result = new PedidoAdapter().AdaptToPedidoItemDetails(pedidoItems);

                ResponseApi responseApi = new ResponseApi();

                responseApi.Data = result;
                responseApi.StatusCode = StatusCodes.Status200OK;

                return responseApi;
            });
        }

    }
}
