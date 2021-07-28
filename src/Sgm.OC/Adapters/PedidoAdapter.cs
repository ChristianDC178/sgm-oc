using System.Collections.Generic;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Models.Requests;
using Sgm.OC.Models.Responses;
using Sgm.OC.Models.Views;

namespace Sgm.OC.Adapters
{
    public class PedidoAdapter
    {

        public List<PedidoView> AdaptToView(List<Pedido> pedidos)
        {
            List<PedidoView> resultado = new List<PedidoView>();

            foreach (var ped in pedidos)
            {
                resultado.Add(AdaptToView(ped));
            }

            return resultado;
        }

        public PedidoView AdaptToView(Pedido ped)
        {
            return new PedidoView
            {
                PedidoId = ped.Id.ToString(),
                Estado = ped.EstadoActual.Descripcion,
                Tipo = ped.Recurrente ? "Recurrente" : "No Recurrente",
                Sucursal = ped.Sucursal.Descripcion,
                Creacion = ped.Creacion.ToString(),
                Usuario = string.Format("{0} {1}", ped.Usuario.Nombre, ped.Usuario.Apellido),
                UsuarioMod = string.Format("{0} {1}", ped.UsuarioModificacion?.Nombre, ped.UsuarioModificacion?.Apellido),
                FechaMod = ped.Modificacion?.ToString()
            };
        }

        public PedidoResponse AdaptToResponse(Pedido pedido)
        {
            List<PedidoItemResponse> items = new List<PedidoItemResponse>();

            foreach (var item in pedido.Items)
            {
                PedidoItemResponse pedidoItemModel = new PedidoItemResponse()
                {
                    Cantidad = item.Cantidad,
                    EnUnidades = item.EnUnidades,
                    Descripcion = item.Producto.Descripcion,
                    ProductoId = item.ProductoId,
                    PedidoItemId = item.Id, 
                    FactorConversion = item.Producto.FactorConversion
                };
                items.Add(pedidoItemModel);
            }

            return new PedidoResponse
            {
                PedidoId = pedido.Id.ToString(),
                PrefijoId = pedido.PrefijoId,
                Recurrente = pedido.Recurrente,
                Estado = pedido.EstadoActual.Descripcion.ToString(),
                Sucursal = pedido.Sucursal.Descripcion,
                Usuario = string.Format("{0} {1}", pedido.Usuario.Nombre, pedido.Usuario.Apellido),
                Creacion = pedido.Creacion,
                Items = items,
                Rechazado = pedido.Rechazado
            };
        }

        public List<PedidoItem> AdaptRequest(PedidoRequest model)
        {

            List<PedidoItem> result = new List<PedidoItem>();

            foreach (var item in model.Items)
            {
                PedidoItem pi = new PedidoItem(
                    item.ProductoId, item.Cantidad, item.EnUnidades
                    );
                pi.Id = item.PedidoItemId;
                result.Add(pi);
            }

            return result;

        }

        public List<PedidoItem> AdaptPedidoItemRequest(List<PedidoItemRequest> list)
        {
            List<PedidoItem> result = new List<PedidoItem>();

            foreach (var item in list)
            {
                PedidoItem pi = new PedidoItem(item.ProductoId, item.Cantidad, item.EnUnidades);
                pi.Id = item.PedidoItemId;
                result.Add(pi);
            }

            return result;

        }

        public List<PedidoItemDetailResponse> AdaptToPedidoItemDetails(List<PedidoItem> pedidoItems)
        {
            List<PedidoItemDetailResponse> result = new List<PedidoItemDetailResponse>();

            pedidoItems.ForEach(pi =>
            {
                result.Add(new PedidoItemDetailResponse()
                {
                    PedidoId = pi.PedidoId,
                    Cantidad = pi.Cantidad,
                    Creacion = pi.Pedido.Creacion.ToShortDateString(),
                    Descripcion = pi.Producto.Descripcion,
                    PedidoItemId = pi.Id,
                    Sucursal = pi.Pedido.Sucursal.Descripcion,
                    Usuario = pi.Pedido.Usuario.Nombre + " " + pi.Pedido.Usuario.Apellido,
                    Recurrente = pi.Pedido.Recurrente
                });
            });

            return result;

        }

    }

}
