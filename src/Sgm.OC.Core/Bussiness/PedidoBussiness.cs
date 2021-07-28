using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Core.Validators;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Filters;
using Sgm.OC.Domain.Views;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Repositories;
using Sgm.OC.Repository.Extensions;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Core.Bussiness
{
    public class PedidoBussiness
    {

        PedidoValidator _pedidoValidator = new PedidoValidator();
        public UserLogged UserLogged { get; set; }

        public void AssingWorkFlow(Pedido pedido, int workflowId)
        {
            UnitOfWork unitOfWork = UnitOfWork.Create();
            Workflow workflow = unitOfWork.WorkflowRepo.GetById(workflowId);
            pedido.Workflow = workflow;
            workflow.Estados = unitOfWork.WorkflowEstadoRepo.DbSet.Where(est => est.WorkflowId == pedido.Workflow.Id).ToList();
            pedido.WorkflowEstados = workflow.GetWorkflowToEntity();
            pedido.WorkflowEstados.First(we => we.Inicial).IsCurrent = true;
        }

        public BusinessResult<Pedido> CreatePedido(bool recurrente, List<PedidoItem> items)
        {

            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                BusinessResult<Pedido> businessResult = new BusinessResult<Pedido>();

                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                Pedido pedido = new Pedido(usuario.SucursalId, usuario.Id);
                pedido.Recurrente = recurrente;
                pedido.Prevalidar = !recurrente;

                AssingWorkFlow(pedido, pedido.Prevalidar ? WorkflowTypeConstants.PRE_VALIDACION : WorkflowTypeConstants.VALIDACION);

                ChangeWorkflowEstadoResult workflowResult = pedido.ApproveWorkflowEstado(usuario, string.Empty);

                List<Producto> productos = unitOfWork.ProductoRepo.DbSet.Where(p => items.Select(i => i.ProductoId).Contains(p.IdInterno)).ToList();

                foreach (var item in items)
                {
                    PedidoItem pedidoItem = new PedidoItem(item.ProductoId, item.Cantidad, item.EnUnidades);
                    pedidoItem.Pedido = pedido;
                    pedidoItem.Producto = productos.First(p => p.IdInterno == item.ProductoId);
                    pedido.Items.Add(pedidoItem);
                }

                businessResult.Validation = _pedidoValidator.Validate(pedido);
                businessResult.Validation.AddRange(workflowResult.Validation.Items);

                if (businessResult.Validation.IsValid)
                {
                    pedido.Items.ForEach(ri =>
                    {
                        unitOfWork.PedidoItemRepo.Create(ri);
                    });

                    pedido.WorkflowEstados.ForEach((est) =>
                    {
                        unitOfWork.WorkflowEstadoEntidadRepo.Create(est);
                    });

                    unitOfWork.PedidoRepo.Create(pedido);

                    unitOfWork.SaveChanges();
                    businessResult.Data = pedido;
                }


                return businessResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Pedido GetById(int id)
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                BusinessResult<bool> businessResult = new BusinessResult<bool>();

                Pedido pedido = unitOfWork.PedidoRepo.GetPedidoById(id);

                return pedido;
            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Fatal("Fallo horrendo al pedir el pedido");
                throw ex;
            }
        }

        public BusinessResult<bool> UpdatePedido(int id, List<PedidoItem> items)
        {
            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                BusinessResult<bool> businessResult = new BusinessResult<bool>();

                Pedido pedido = unitOfWork.PedidoRepo.GetPedidoById(id);
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);
                PedidoItem found = null;

                foreach (PedidoItem peditem in items)
                {
                    found = pedido.Items.FirstOrDefault(x => x.Id == peditem.Id);

                    if (found != null)
                    {
                        found.Cantidad = peditem.Cantidad;
                        found.EnUnidades = peditem.EnUnidades;
                        unitOfWork.PedidoItemRepo.Update(found);
                    }
                    else
                    {
                        businessResult.Validation.Add($"Item {peditem.Id} no esta en el pedido.");


                    }

                }

                if (businessResult.Validation.IsValid)
                {
                    pedido.Modificacion = DateTime.Now;
                    pedido.UsuarioModificacion = usuario;


                    unitOfWork.SaveChanges();
                    businessResult.Data = true;
                }

                return businessResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ViewResult<PedidoView> GetPedidoByFilter(PedidoFilters filtro)
        {
            ViewResult<PedidoView> result = new ViewResult<PedidoView>();

            if ((filtro.Hasta.HasValue && filtro.Desde.HasValue) && (filtro.Hasta.Value < filtro.Desde.Value))
            {
                result.Validation.Add("El filtro 'Desde' no puede ser mayor a 'Hasta'");
                return result;

            }



            int minPageSize = 10;
            int maxPageSize = 50;
            int minPageNumber = 1;

            int pagesize = minPageSize;
            int pageNumber = minPageNumber;

            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                if (!string.IsNullOrEmpty(filtro.Tipo))
                {
                    filtro.Tipo = filtro.Tipo == "Recurrente" ? "1" : "0";
                }

                if (!UserLogged.IsInRole("aco", "gcc"))

                {
                    filtro.Sucursal = UserLogged.SucursalId.ToString();
                }

                if (UserLogged.IsInRole("sol") && UserLogged.Roles.Count == 1)
                {
                    var usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);
                    filtro.UsuarioId = usuario.Id;

                }


                List<PedidoView> pedidos = new List<PedidoView>();

                pedidos = unitOfWork.PedidoViewRepo.GetPedidoByFilter(filtro);

                if (pedidos.Any())
                {
                    return new ViewResult<PedidoView>(pedidos, pedidos.First().TotalPages, pedidos.First().TotalRows);

                }

                return new ViewResult<PedidoView>(pedidos, 0, 0);
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Fatal("Error al crear requisición", ex);
                throw ex;
            }

        }

        public bool IsNumeric(string str)
        {
            if (int.TryParse(str, out int salida))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ViewResult<PedidoItemView> GetPedidoItemsByFilter(PedidoItemViewFilters filters)
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                var items = unitOfWork.PedidoItemViewRepo.GetPedidoItemsByFilter(filters);

                if (items.Any())
                {
                    if (!string.IsNullOrEmpty(filters.NotPedidoItemIds))
                    {
                        var notIds = filters.NotPedidoItemIds.Split(",").ToList();

                        List<int> notIdsInt = new List<int>();

                        notIds.ForEach(nid => {
                            notIdsInt.Add(int.Parse(nid));
                        });

                        items = items.Where(i => !notIdsInt.Contains(i.PedidoItemId)).ToList();
                    }

                    if (!items.Any())
                    {
                        return new ViewResult<PedidoItemView>(items, 0, 0);
                    }

                    items = items.Select(pedidoItemView =>
                    {
                        if (string.IsNullOrEmpty(pedidoItemView.RequisicionId))
                        {
                            pedidoItemView.RequisicionId = "No asignado";
                        }

                        return pedidoItemView;
                    }).ToList();

                    return new ViewResult<PedidoItemView>(items, items.First().TotalPages, items.First().TotalRows);
                }

                return new ViewResult<PedidoItemView>(items, 0, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Workflo
        public BusinessResult<List<WorkflowEstadoEntidad>> GetPedidoWorkflow(int pedidoId)
        {
            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                Pedido pedido = unitOfWork.PedidoRepo.GetById(pedidoId);
                BusinessResult<List<WorkflowEstadoEntidad>> businessResult = new BusinessResult<List<WorkflowEstadoEntidad>>();

                if (pedido == null)
                    businessResult.Validation.Add($"La requisicion {pedidoId} no tiene un workflow");

                if (businessResult.Validation.IsValid)
                {
                    pedido.WorkflowEstados = unitOfWork.WorkflowEstadoEntidadRepo.DbSet
                                                    .Where(we => we.PedidoId == pedido.Id)
                                                    .Include(we => we.Estado)
                                                    .OrderBy(we => we.Secuencia)
                                                    .ToList();

                    businessResult.Data = pedido.WorkflowEstados;
                }

                return businessResult;

            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Error($"Error al traer el workflow de una entidad {ex.Message}");
                throw ex;
            }
        }

        public BusinessResult<ChangeWorkflowEstadoResult> ChangeEstado(int pedidoId, string comentario, bool rechazar)
        {
            try
            {

                SgmApplication.Logger.Info($"Aprobando la requisicon {pedidoId}");

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                BusinessResult<ChangeWorkflowEstadoResult> businessResult = new BusinessResult<ChangeWorkflowEstadoResult>();
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                Pedido pedido = unitOfWork.PedidoRepo.GetById(pedidoId);
                ChangeWorkflowEstadoResult result = null;

                if (pedido == null)
                {
                    businessResult.Validation.Add($"El pedido {pedidoId} no existe");
                    return businessResult;
                }

                pedido.WorkflowEstados =
                        unitOfWork.WorkflowEstadoEntidadRepo.DbSet.Where(req => req.PedidoId == pedidoId)
                        .Include(we => we.Estado)
                        .ToList();

                if (rechazar)
                {
                    result = pedido.RejectWorkFlowEstado(usuario, comentario);
                }
                else
                {
                    result = pedido.ApproveWorkflowEstado(usuario, comentario);
                }

                if (!result.Validation.IsValid)
                {
                    SgmApplication.Logger.Info($"No se pudo cambiar el estado del pedido {pedido.Id}");
                    businessResult.Validation.AddRange(result.Validation.Items);
                    return businessResult;
                }

                if (result.EstadoSiguiente != null)
                    unitOfWork.WorkflowEstadoEntidadRepo.Update(result.EstadoSiguiente);

                unitOfWork.WorkflowEstadoEntidadRepo.Update(result.EstadoAprobado);
                unitOfWork.PedidoRepo.Update(pedido);
                unitOfWork.SaveChanges();

                result.WorkflowEstados = pedido.WorkflowEstados;
                businessResult.Data = result;

                return businessResult;

            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Error($"Error al cambiar el estado del pedido {pedidoId}");
                throw ex;
            }
            #endregion
        }
    }
}