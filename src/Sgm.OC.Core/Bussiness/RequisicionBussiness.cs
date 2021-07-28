using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Core.Validators;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Views;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Repositories;
using Sgm.OC.Repository.Extensions;
using Sgm.OC.Security.Entities;
using Sgm.OC.Domain;

namespace Sgm.OC.Core.Bussiness
{
    public class RequisicionBussiness
    {

        RequisicionValidator _requisicionValidator = new RequisicionValidator();

        public UserLogged UserLogged { get; set; }

        public void AssingWorkFlow(Requisicion requisicion, int workflowId)
        {
            UnitOfWork unitOfWork = UnitOfWork.Create();
            Workflow workflow = unitOfWork.WorkflowRepo.GetById(workflowId);
            requisicion.Workflow = workflow;
            workflow.Estados = unitOfWork.WorkflowEstadoRepo.DbSet.Where(est => est.WorkflowId == requisicion.Workflow.Id).ToList();
            requisicion.WorkflowEstados = workflow.GetWorkflowToEntity();
            requisicion.WorkflowEstados.First(we => we.Inicial).IsCurrent = true;
        }

        public List<Sucursal> GetSucursales()
        {
            try
            {
                UnitOfWork unitOfWork = UnitOfWork.Create();
                return unitOfWork.SucursalRepo.DbSet.ToList();
            } catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Ocurrio un error al obtener las sucursales", ex);
                throw ex;
            }
        }

        public BusinessResult<int> CreateRequisicion(RequisicionRequest requisicionRequest)
        {

            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();

                BusinessResult<int> businessResult = new BusinessResult<int>();
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                Requisicion requisicion = new Requisicion(usuario.SucursalId, usuario.Id);
                requisicion.EstadoId = 5; //TODO 

                if (usuario.Login == "sistema")
                {
                    requisicion.Tipo = "Auto";
                }

                AssingWorkFlow(requisicion, WorkflowTypeConstants.REQUISICION);

                List<int> pedItemsIds = new List<int>();

                requisicionRequest.Items.ForEach(item =>
                {
                    pedItemsIds.AddRange(item.PedidoItemIds);
                });

                List<PedidoItem> lsPedidoItems = unitOfWork.PedidoItemRepo.DbSet
                                                    .Include(pi => pi.Producto)
                                                  .Where(pi => pedItemsIds.Contains(pi.Id))
                                                  .ToList();

                foreach (var item in requisicionRequest.Items)
                {
                    RequisicionItem requisicionItem = new RequisicionItem(item.ProductoId, item.Cantidad, item.EnUnidades);
                    requisicionItem.Requisicion = requisicion;
                    requisicion.Items.Add(requisicionItem);
                }

                requisicion.Recurrente = lsPedidoItems.First().Producto.Recurrente;

                businessResult.Validation = _requisicionValidator.Validate(requisicion);

                if (businessResult.Validation.IsValid)
                {
                    requisicion.Items.ForEach(ri =>
                    {
                        unitOfWork.RequisicionItemRepo.Create(ri);
                    });

                    requisicion.WorkflowEstados.ForEach((est) =>
                    {
                        unitOfWork.WorkflowEstadoEntidadRepo.Create(est);
                    });

                    lsPedidoItems.ForEach((pi) =>
                    {
                        pi.Requisicion = requisicion;
                        unitOfWork.PedidoItemRepo.Update(pi);
                    });

                    unitOfWork.RequisicionRepo.Create(requisicion);
                    unitOfWork.SaveChanges();
                    businessResult.Data = requisicion.Id;
                }

                return businessResult;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Ocurrio un error al crear la requisicion", ex);
                throw ex;
            }
        }

        public ViewResult<RequisicionView> GetRequisicionByFilter(RequisicionFilters filtro)
        {
            ViewResult<RequisicionView> result = new ViewResult<RequisicionView>();

            if ((filtro.Hasta.HasValue && filtro.Desde.HasValue) && (filtro.Hasta.Value < filtro.Desde.Value))
            {
                result.Validation.Add("El filtro 'Desde' no puede ser mayor a 'Hasta'");
                return result;
            }



            int minPageSize = 10;
            int maxPageSize = 50;
            int minPageNumber = 1;

            int pagesize;//= minPageSize;
            int pageNumber = minPageNumber;

            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                if (int.TryParse(filtro.PageSize, out pagesize))
                {
                    pagesize = int.Parse(filtro.PageSize);
                }
                if (pagesize < minPageSize || pagesize > maxPageSize)
                {
                    pagesize = minPageSize;
                }
                filtro.PageSize = pagesize.ToString();

                if (int.TryParse(filtro.Page, out pageNumber))
                {
                    pageNumber = int.Parse(filtro.Page);

                }
                if (pageNumber < minPageNumber)
                {
                    pageNumber = minPageNumber;
                }
                filtro.Page = pageNumber.ToString();

                List<RequisicionView> requisiciones = new List<RequisicionView>();

                if (filtro.Modo == "ped")
                {
                    requisiciones = unitOfWork.RequisicionViewRepo.GetRequisicionByPedido(filtro);
                }
                else
                {
                    requisiciones = unitOfWork.RequisicionViewRepo.GetRequisicionByFilter(filtro);
                }

                result.Items = requisiciones;
                var req = requisiciones.FirstOrDefault();
                if (req != null)
                {
                    result.PageCount = req.TotalPages;
                    result.RowCount = req.TotalRows;
                }
                return result;
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Fatal("Error al crear requisición", ex);
                throw ex;
            }
        }

        public Requisicion GetById(int id)
        {
            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                Requisicion requisicion = unitOfWork.RequisicionRepo.DbSet
                    .Include(req => req.EstadoActual)
                    .Include(req => req.Items)
                        .ThenInclude(item => item.Producto)
                    .Include(req => req.Sucursal)
                    .Include(req => req.Usuario)
                    .Include(req => req.SucursalAEntregar)
                    .FirstOrDefault(req => req.Id == id);

                return requisicion;
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Error($"Error obteniendo la requisicion {id}");
                throw ex;
            }
        }

        public List<PedidoItem> GetPedidoItemsByRequisicionId(int requisicionId, int productoId)
        {
            try
            {
                var unitOfWork = UnitOfWork.Create();
                List<PedidoItem> items = unitOfWork.PedidoItemRepo.GetByRequisicionId(requisicionId);
                return items.Where(i => i.ProductoId == productoId).ToList();
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Error($"Error obteniendo los detalles de la requisicion {requisicionId}", ex);
                throw;
            }
        }

        #region Workflow

        public BusinessResult<List<WorkflowEstadoEntidad>> GetRequisicionWorkflow(int requisicionId)
        {
            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                Requisicion requisicion = unitOfWork.RequisicionRepo.GetById(requisicionId);
                BusinessResult<List<WorkflowEstadoEntidad>> businessResult = new BusinessResult<List<WorkflowEstadoEntidad>>();

                if (requisicion == null)
                    businessResult.Validation.Add($"La requisicion {requisicionId} no tiene un workflow");

                if (businessResult.Validation.IsValid)
                {
                    requisicion.WorkflowEstados = unitOfWork.WorkflowEstadoEntidadRepo.DbSet
                                                    .Where(we => we.RequisicionId == requisicion.Id)
                                                    .Include(we => we.Estado)
                                                    .OrderBy(we => we.Secuencia)
                                                    .ToList();

                    businessResult.Data = requisicion.WorkflowEstados;
                }

                return businessResult;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error($"Error al traer el workflow de una entidad {ex.Message}");
                throw ex;
            }
        }

        public BusinessResult<ChangeWorkflowEstadoResult> ChangeEstado(int requisicionId, string comentario, bool rechazar)
        {
            try
            {

                SgmApplication.Logger.Info($"Cambiando el estado de la requisicion {requisicionId}");

                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                BusinessResult<ChangeWorkflowEstadoResult> businessResult = new BusinessResult<ChangeWorkflowEstadoResult>();
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                Requisicion requisicion = unitOfWork.RequisicionRepo.GetRequisicionById(requisicionId);
                ChangeWorkflowEstadoResult result = null;

                if (requisicion == null)
                {
                    businessResult.Validation.Add($"El pedido {requisicionId} no existe");
                    return businessResult;
                }

                requisicion.WorkflowEstados =
                        unitOfWork.WorkflowEstadoEntidadRepo.DbSet.Where(req => req.RequisicionId == requisicionId)
                        .Include(we => we.Estado)
                        .ToList();

                requisicion.Presupuestos = unitOfWork.PresupuestoRepo.DbSet.Where(p => p.RequisicionId == requisicion.Id).ToList();

                if (rechazar)
                {
                    result = requisicion.RejectWorkFlowEstado(usuario, comentario);
                }
                else
                {

                    if (requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.EnPresupuestacion && !requisicion.Presupuestos.Any())
                    {
                        businessResult.Validation.Add("La requisición debe tener presupuestos.");
                        return businessResult;
                    }
                    if ((requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.EnPresupuestacion && !requisicion.CotizacionPedida) ||
                       (requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.EnPresupuestacion && requisicion.CotizacionPedida && requisicion.Presupuestos.Any(p => !p.Cotizacion.HasValue)))
                    {
                        businessResult.Validation.Add("Deben estar todas las cotizaciones cargadas.");
                        return businessResult;
                    }
                    if (requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.Presupuestado && requisicion.Presupuestos.FirstOrDefault(P => !P.Cotizacion.HasValue) != null)
                    {
                        businessResult.Validation.Add("Los presupuestos necesitan estar cotizados.");
                        return businessResult;
                    }
                    if (requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.Aceptado && requisicion.Presupuestos.FirstOrDefault(P => P.Aprobado) == null)
                    {
                        businessResult.Validation.Add("Debe haber un presupuesto aprobado.");
                        return businessResult;
                    }

                    if (requisicion.EstadoId == Sgm.OC.Domain.RequisicionEstadosConstants.Aceptado)
                    {
                        if (requisicion.Items.FirstOrDefault(i => i.Producto.Precio != i.Precio) != null)
                        {
                            businessResult.Validation.Add("Existen precios que son distintos en SGM.");
                            return businessResult;
                        }

                        if (requisicion.Presupuestos.First(p => p.Aprobado).Cotizacion != requisicion.Items.Sum(ri => ri.Precio))
                        {
                            businessResult.Validation.Add("El total de la OC a emitir es distinta al monto presupuestado.");
                            return businessResult;
                        }
                    }
                    result = requisicion.ApproveWorkflowEstado(usuario, comentario);
                }

                if (!result.Validation.IsValid)
                {
                    SgmApplication.Logger.Info($"No se pudo cambiar el estado de la requisicion {requisicion.Id}");
                    businessResult.Validation.AddRange(result.Validation.Items);
                    return businessResult;
                }

                if (result.EstadoSiguiente != null)
                    unitOfWork.WorkflowEstadoEntidadRepo.Update(result.EstadoSiguiente);

                unitOfWork.WorkflowEstadoEntidadRepo.Update(result.EstadoAprobado);
                unitOfWork.RequisicionRepo.Update(requisicion);
                unitOfWork.SaveChanges();

                result.WorkflowEstados = requisicion.WorkflowEstados;
                businessResult.Data = result;

                return businessResult;

            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Error($"Error al cambiar el estado de la requisicion {requisicionId}");
                throw ex;
            }
        }

        #endregion

    }
}
