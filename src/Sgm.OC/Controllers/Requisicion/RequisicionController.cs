using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sgm.OC.Adapters;
using Sgm.OC.Core;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Models.Requests;
using Sgm.OC.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using HttpStatus = Microsoft.AspNetCore.Http.StatusCodes;

namespace Sgm.OC.Controllers
{
    [ApiController]
    [Route("api/requisiciones")]
    public class RequisicionController : SgmController
    {

        ILogger<RequisicionController> _logger;
        RequisicionBussiness _requisicionBussiness;

        public RequisicionController(ILogger<RequisicionController> logger)
        {
            _logger = logger;
            _requisicionBussiness = new RequisicionBussiness();
        }

        [HttpPost]
        public IActionResult CreateRequiscion(Models.Requests.RequisicionRequest model)
        {
            return ExecuteResponse(() =>
            {

                Domain.RequisicionRequest requisicionRequest = new RequisicionAdapter().AdaptRequest(model);
                _requisicionBussiness.UserLogged = UserLogged;

                BusinessResult<int> result = _requisicionBussiness.CreateRequisicion(requisicionRequest);

                ResponseApi responseApi = new ResponseApi();

                if (result.Validation.IsValid)
                {
                    responseApi.StatusCode = HttpStatus.Status201Created;
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
        [Route("{requisicionId}/estados")]
        public IActionResult SetNewEstado(ChangeEstadoRequest request, int requisicionId)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                requisicionBussiness.UserLogged = UserLogged;

                WorkflowEstadoEntidadAdapter adapter = new WorkflowEstadoEntidadAdapter();
                BusinessResult<ChangeWorkflowEstadoResult> businessResult = requisicionBussiness.ChangeEstado(requisicionId, request.Comentario, request.Rechazar);

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

        #region Presupuestos

        /// <summary>
        /// Agrega un presupuesto en la requisicion. Solamente asocia Requisicion con el proveedor.
        /// </summary>
        /// <param name="requisicionId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{requisicionId}/presupuestos")]
        public IActionResult CreatePresupuesto(int requisicionId, CreatePresupuestoRequest request)
        {
            return ExecuteResponse(() =>
            {
                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                presupuestoBussiness.UserLogged = UserLogged;

                BusinessResult<int> result = presupuestoBussiness.CreatePresupuesto(requisicionId, request.ProveedorIdInterno);

                ResponseApi responseApi = new ResponseApi();

                if (result.Validation.IsValid)
                {
                    responseApi.Data = result.Data;
                    responseApi.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    responseApi.Validations = result.Validation.GetMessages();
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                }

                return responseApi;

            });
        }

        /// <summary>
        /// Actualiza el presupuesto con el valor de la cotizacion y el archivo pdf
        /// </summary>
        /// <param name="requisicionId"></param>
        /// <param name="presupuestoId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{requisicionId}/presupuestos/{presupuestoId}")]
        public IActionResult AddCotizacion(int requisicionId, int presupuestoId)
        {
            return ExecuteResponse(() =>
            {
                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                presupuestoBussiness.UserLogged = UserLogged;

                ResponseApi responseApi = new ResponseApi();

                decimal monto = 0;
                string fileName = $"presupuesto_{presupuestoId}";

                if (Request.Form.ContainsKey("cotizacion") && (Request.Form.TryGetValue("cotizacion", out Microsoft.Extensions.Primitives.StringValues montoAux)))
                {
                    Sgm.OC.Framework.SgmApplication.Logger.Debug("Contienen la cotizacion");
                    monto = decimal.Parse(montoAux.ToString());
                }


                if (Request.Form.ContainsKey("archivoNombre"))
                {
                    Sgm.OC.Framework.SgmApplication.Logger.Debug("contiene el archivo");

                }


                if (Request.Form.Files != null)
                {

                    byte[] fileBytesArray = getBytesFromFile(Request.Form.Files[0]);

                    BusinessResult<bool> result =
                            presupuestoBussiness.AddCotizacion(requisicionId, presupuestoId, monto, fileName, fileBytesArray);

                    if (result.Validation.IsValid)
                    {
                        crearArchivoPresupuesto(fileBytesArray, requisicionId, presupuestoId);

                        responseApi.Data = result.Data;
                        responseApi.StatusCode = StatusCodes.Status202Accepted;
                    }
                    else
                    {
                        responseApi.Validations = result.Validation.GetMessages();
                        responseApi.StatusCode = StatusCodes.Status404NotFound;
                    }
                }


                return responseApi;

            });
        }


        private byte[] getBytesFromFile(Microsoft.AspNetCore.Http.IFormFile formFile)
        {
            byte[] array = null;
            var tempFilePath = Path.GetTempFileName();

            Sgm.OC.Framework.SgmApplication.Logger.Debug("archivo alojado en " + tempFilePath);

            if (formFile.Length > 0)
            {
                using (var inputStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    // read file to stream
                    formFile.CopyTo(inputStream);
                    // stream to byte arrays
                    array = new byte[inputStream.Length];
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.Read(array, 0, array.Length);
                }
            }

            return array;
        }

        private void crearArchivoPresupuesto(byte[] fileBytesArray, int requisicionId, int presupuestoId)
        {
            if (!Directory.Exists(Sgm.OC.Framework.Settings.FilePath))
                Directory.CreateDirectory(Sgm.OC.Framework.Settings.FilePath);

            string relativePath = System.IO.Path.Combine(Sgm.OC.Framework.Settings.FilePath, $"Requisicion_{requisicionId}");

            if (!Directory.Exists(relativePath))
                Directory.CreateDirectory(relativePath);

            string filePath = System.IO.Path.Combine(relativePath, "presupuesto_" + presupuestoId + ".txt");

            System.IO.File.WriteAllText(filePath, "data:application/pdf;base64," + Convert.ToBase64String(fileBytesArray));
        }

        [HttpGet]
        [Route("{requisicionId}/presupuestos/{presupuestoId}/archivos")]
        public IActionResult GetPresupuestoArchivo(int requisicionId, int presupuestoId)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();
                var sett = new Sgm.OC.Framework.Settings();
                var filePath = System.IO.Path.Combine(Sgm.OC.Framework.Settings.FilePath, $"Requisicion_{requisicionId}", "presupuesto_" + presupuestoId + ".txt");
                responseApi.Data = System.IO.File.ReadAllText(filePath);
                responseApi.StatusCode = StatusCodes.Status200OK;
                return responseApi;
            });
        }

        /// <summary>
        /// Aprueba el presupuesto enviado como parametro, para mayor seguridad y validaciones se agrega el id de la requisicion.
        /// </summary>
        /// <param name="requisicionId"></param>
        /// <param name="presupuestoId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{requisicionId}/presupuestos/{presupuestoId}/aprobaciones")]
        public IActionResult ApprovePresupuesto(int requisicionId, int presupuestoId, [FromBody] ApprovePresupuestoRequest approveRequest)
        {
            return ExecuteResponse(() =>
            {

                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                presupuestoBussiness.UserLogged = UserLogged;
                ResponseApi responseApi = new ResponseApi();

                List<RequisicionItemPrecio> precios = new List<RequisicionItemPrecio>();

                //temporal pre demo 
                foreach (var item in approveRequest.Items)
                {
                    precios.Add(new RequisicionItemPrecio()
                    {
                        RequisicionItemId = item.RequisicionItemId,
                        Precio = item.Precio
                    });
                }

                BusinessResult<bool> businessResult = presupuestoBussiness.Approve(requisicionId, presupuestoId, precios, approveRequest.ComentarioAprobacion, approveRequest.SucursalAEntregar);

                if (businessResult.Validation.IsValid)
                {
                    responseApi.StatusCode = StatusCodes.Status202Accepted;
                    responseApi.Data = businessResult.Data;
                }
                else
                {
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = businessResult.Validation.GetMessages();
                }

                return responseApi;

            });
        }

        /// <summary>
        /// Envia los emails a los proveedores que estan en el listado de presupuestos.
        /// </summary>
        /// <param name="requisicionId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{requisicionId}/presupuestos/cotizaciones")]
        public IActionResult RequestCotizacion(int requisicionId)
        {
            return ExecuteResponse(() =>
            {

                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                BusinessResult<bool> result = presupuestoBussiness.RequestCotizacion(requisicionId);
                ResponseApi responseApi = new ResponseApi();

                if (result.Validation.IsValid)
                {
                    responseApi.Data = result.Data;
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = result.Validation.GetMessages();
                }

                return responseApi;
            });
        }

        [HttpDelete]
        [Route("{requisicionId}/presupuestos/{presupuestoId}")]
        public IActionResult DeletePresupuesto(int requisicionId, int presupuestoId)
        {
            return ExecuteResponse(() =>
            {

                PresupuestoBussiness presupuestoBussiness = new PresupuestoBussiness();
                ResponseApi responseApi = new ResponseApi();

                presupuestoBussiness.UserLogged = UserLogged;
                BusinessResult<List<Presupuesto>> result = presupuestoBussiness.DeletePresupuesto(requisicionId, presupuestoId);

                if (result.Validation.IsValid)
                {
                    List<PresupuestoResponse> lsPresupuestosResponses = new PresupuestoAdapter().Adapt(result.Data);
                    responseApi.Data = lsPresupuestosResponses;
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    responseApi.Validations = result.Validation.GetMessages();
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                }

                return responseApi;

            });
        }



        #endregion

    }
}


