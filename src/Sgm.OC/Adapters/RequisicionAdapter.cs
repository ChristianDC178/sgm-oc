using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Requests;
using Sgm.OC.Models.Responses;
using Sgm.OC.Models.Views;
using Sgm.OC.Domain;
using Microsoft.OpenApi.Models;

namespace Sgm.OC.Adapters
{
    public class RequisicionAdapter
    {

        public List<RequisicionView> AdaptToView(List<Requisicion> requisiciones)
        {
            List<RequisicionView> resultado = new List<RequisicionView>();

            foreach (var req in requisiciones)
            {
                resultado.Add(AdaptToView(req));
            }

            return resultado;
        }

        public RequisicionView AdaptToView(Requisicion req)
        {
            return new RequisicionView
            {
                RequisicionId = req.Id.ToString(),
                Estado = req.EstadoActual.Descripcion,
                Tipo = req.Recurrente ? "Recurrente" : "No Recurrente",
                Sucursal = req.Sucursal.Descripcion,
                Creacion = req.Creacion.ToString(),
                Usuario = string.Format("{0} {1}", req.Usuario.Nombre, req.Usuario.Apellido),
                Modificacion = req.Modificacion.ToString()
            };
        }

        public RequisicionResponse AdaptToResponse(Requisicion req)
        {
            List<RequisicionItemResponse> items = new List<RequisicionItemResponse>();
            foreach (var item in req.Items)
            {

                RequisicionItemResponse requisicionItemModel = new RequisicionItemResponse()
                {
                    RequisicionItemId = item.Id,
                    Cantidad = item.Cantidad,
                    EnUnidades = item.EnUnidades,
                    Descripcion = item.Producto.Descripcion,
                    ProductoId = item.ProductoId,
                    PrecioSgm = item.Producto.Precio,
                    Precio = item.Precio,
                    FactorConversion = item.Producto.FactorConversion
                };

                if (item.Precio.HasValue && item.Producto.Precio.HasValue)
                {
                    requisicionItemModel.PrecioIsValid = item.Precio == item.Producto.Precio;
                }

                items.Add(requisicionItemModel);

            }

            return new RequisicionResponse
            {
                RequisicionId = req.Id.ToString(),
                PrefijoId = req.PrefijoId,
                Recurrente = req.Recurrente,
                Estado = req.EstadoActual.Descripcion.ToString(),
                EstadoId = req.EstadoId,
                Sucursal = req.Sucursal.Descripcion,
                Usuario = string.Format("{0} {1}", req.Usuario.Nombre, req.Usuario.Apellido),
                Creacion = req.Creacion,
                CotizacionPedida = req.CotizacionPedida,
                ComentarioAprobacion = req.ComentarioAprobacion,
                Items = items,
                NroOC = req.OrdenCompraKey.NroOC,
                SucursalAEntregar = req.SucursalAEntregar?.Descripcion
            };
        }

        public Domain.RequisicionRequest AdaptRequest(Models.Requests.RequisicionRequest model)
        {
            Domain.RequisicionRequest domainRequest = new Domain.RequisicionRequest();
            domainRequest.Recurrente = model.Recurrente;

            foreach (var item in model.Items)
            {
                domainRequest.Items.Add(new Domain.RequisicionItemRequest()
                {
                    Cantidad = item.Cantidad,
                    EnUnidades = item.EnUnidades,
                    PedidoItemIds = item.PedidoItemIds,
                    ProductoId = item.ProductoId
                });
            }

            return domainRequest;
        }

    }

}
