using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgm.OC.Adapters;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Views;
using Sgm.OC.Models.Validator;
using Org.BouncyCastle.Crypto;
using Sgm.OC.Models.Responses;

namespace Sgm.OC.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController : SgmController
    {

        ProductoAdapter productoAdapter = new ProductoAdapter();
        ProductoBusiness _porductoBussiness = new ProductoBusiness();
        ProductoFiltersValidator productoValidator = new ProductoFiltersValidator();
        ILogger<ProductosController> _logger;

        public ProductosController(
            ILogger<ProductosController> logger)
        {
            _logger = logger;
            _porductoBussiness = new ProductoBusiness();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProducto(int id)
        {
            return ExecuteResponse(() =>
            {
                _logger.LogDebug("Obteniendo Producto {0}", id);

                ResponseApi responseApi = new ResponseApi();
                ProductoAdapter productoAdapter = new ProductoAdapter();
                Producto producto = _porductoBussiness.GetProducto(id);

                if (producto != null && producto.Id > 0)
                {
                    _logger.LogInformation("Productos encontrado {0}", producto.Descripcion);
                    responseApi.Data = productoAdapter.Adapt(producto);
                    responseApi.StatusCode = StatusCodes.Status302Found;
                }
                else
                {
                    _logger.LogInformation("No se encontro ningun producto");
                    responseApi.StatusCode = StatusCodes.Status404NotFound;
                }

                return responseApi;
            });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetProductoByFilters([FromQuery] ProductoFilters filtro)
        {

            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();

                _logger.LogInformation("Buscando productos");

                ProductoAdapter productoAdapter = new ProductoAdapter();

                ValidationResult validationResult = productoValidator.Validate(filtro);

                if (validationResult.IsValid)
                {
                    List<Producto> productos = _porductoBussiness.GetProductoByFilter(filtro);
                    List<ProductoView> productoViews = productoAdapter.Adapt(productos);

                    _logger.LogInformation("Fin de productos (Valido)");

                    responseApi.Data = productoViews;
                    responseApi.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    _logger.LogInformation("Fin de productos (No valido)");
                    responseApi.StatusCode = StatusCodes.Status400BadRequest;
                    responseApi.Validations = validationResult.Errors.Select(er => er.ErrorMessage).ToList();
                }

                return responseApi;

            });
        }


        [HttpGet]
        [Route("proveedor/{proveedorId}/precios")]
        public IActionResult GetPrecios(int proveedorId, [FromQuery] int[] productoId)
        {
            return ExecuteResponse(() =>
            {
                ResponseApi responseApi = new ResponseApi();

                List<ProductoPrecioResponse> precios = new List<ProductoPrecioResponse>();

                foreach (var item in productoId)
                {
                    precios.Add(new ProductoPrecioResponse() { ProductoId = item, ProveedorId = proveedorId, PrecioSGM = 500  });
                }

                responseApi.Data = precios;
                responseApi.StatusCode = StatusCodes.Status200OK;
                return responseApi;
            });
        }


    }
}
