using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Framework;
using Sgm.OC.Repositories;
using Sgm.OC.Repository.Extensions;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Core.Bussiness
{
    public class PresupuestoBussiness
    {

        public UserLogged UserLogged { get; set; }

        public BusinessResult<int> CreatePresupuesto(int requisicionId, int proveedorIdInterno)
        {
            try
            {

                BusinessResult<int> result = new BusinessResult<int>();
                UnitOfWork unitOfWork = UnitOfWork.Create();
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                if (!usuario.IsInRole("aco"))
                {
                    result.Validation.Add("No tiene permisos para cargar presupuestos");
                    return result;
                }

                Requisicion requisicion = unitOfWork.RequisicionRepo.DbSet.FirstOrDefault(req => req.Id == requisicionId);

                if (requisicion == null)
                {
                    result.Validation.Add("Nro de requisición inválido");
                    return result;
                }

                Proveedor proveedor = unitOfWork.ProovedorRepo.DbSet.FirstOrDefault(p => p.IdInterno == proveedorIdInterno);

                if (proveedor == null)
                {
                    result.Validation.Add("Proveedor inválido");
                    return result;
                }

                if (requisicion.EstadoId != 5) //No hay que hacer esto, magic numbers
                {
                    result.Validation.Add("La requisicion debe estar en estado 'En Presupuestación'");
                    return result;
                }

                Presupuesto existentPresupuesto = unitOfWork.PresupuestoRepo.GetByRequisicionProveedorId(requisicionId, proveedor.Id);
                if (existentPresupuesto != null)
                {
                    result.Validation.Add($"Ya existe un presupuesto para el proveedor {proveedor.Descripcion}");
                    return result;
                }

                if (result.Validation.IsValid)
                {
                    Presupuesto presupuesto = new Presupuesto()
                    {
                        UsuarioId = usuario.Id,
                        ProveedorId = proveedor.Id,
                        RequisicionId = requisicion.Id
                    };

                    unitOfWork.PresupuestoRepo.Create(presupuesto);
                    unitOfWork.SaveChanges();

                    result.Data = presupuesto.Id;
                }

                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Presupuesto> GetPresupuestosByRequisicion(int requisicionId)
        {
            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();
                List<Presupuesto> presupuestos = unitOfWork.PresupuestoRepo.DbSet
                                                    .Where(p => p.RequisicionId == requisicionId)
                                                    .Include(p => p.Proveedor)
                                                    .OrderBy(p => p.Cotizacion)
                                                    .ToList();
                return presupuestos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BusinessResult<bool> AddCotizacion(int requisicionId, int presupuestoId, decimal monto, string archivoNombre, byte[] archivoData)
        {
            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();
                BusinessResult<bool> result = new BusinessResult<bool>();
                Presupuesto presupuesto = unitOfWork.PresupuestoRepo.DbSet.FirstOrDefault(p => p.Id == presupuestoId);

                presupuesto.Cotizacion = monto;

                presupuesto.Archivo = new Archivo();
                presupuesto.Archivo.ArchivoId = Guid.NewGuid();
                presupuesto.Archivo.Nombre = archivoNombre;
                presupuesto.Archivo.Path = System.IO.Path.Combine(Sgm.OC.Framework.Settings.FilePath, $"Requisicion_{requisicionId}", "presupuesto_" + presupuestoId + ".txt");

                if (result.Validation.IsValid)
                {
                    unitOfWork.PresupuestoRepo.Update(presupuesto);
                    unitOfWork.SaveChanges();
                    //Save file to disk
                    result.Data = true;
                }

                return result;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Ocurrio un error al guardar el archivo", ex);
                throw ex;
            }
        }

        public BusinessResult<bool> RequestCotizacion(int requisicionId)
        {
            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();
                BusinessResult<bool> result = new BusinessResult<bool>();
                MailSender mailSender = new MailSender();
                DocumentGenerator documentGenerator = new DocumentGenerator();

                Requisicion requisicion = unitOfWork.RequisicionRepo.DbSet
                    .Include(r => r.Items)
                        .ThenInclude(i => i.Producto)
                    .FirstOrDefault(r => r.Id == requisicionId);

                if (requisicion == null)
                {
                    result.Validation.Add("No existe la requisición");
                    return result;
                }

                if (requisicion.EstadoId != RequisicionEstadosConstants.EnPresupuestacion)
                {
                    result.Validation.Add("La requisición debe estar en Presupuestación");
                    return result;
                }

                if (requisicion.CotizacionPedida)
                {
                    result.Validation.Add("La cotización ya fue pedida");
                    return result;
                }

                List<Presupuesto> presupuestos = unitOfWork.PresupuestoRepo.DbSet
                                                    .Where(p => p.RequisicionId == requisicionId)
                                                    .Include(p => p.Proveedor)
                                                    .ToList();

                if (presupuestos.Count == 0)
                {
                    result.Validation.Add("La requisicion no tiene presupuestos");
                    return result;
                }


                if (result.Validation.IsValid)
                {
                    //byte[] file = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Environment.CurrentDirectory, "diarco1.pdf"));

                    byte[] file = documentGenerator.CreateRequisicionDocument(requisicion);

                    presupuestos.ForEach((p) =>
                    {
                        string body = "<div>   " +
                                          "<p> Estimados </p>    " +
                                          "<p style = 'line-height: 0.1;padding-left:50px;'> Mediante el presente solicitamos por favor vuestra cotización por los siguientes ítems y cantidades, indicados en el archivo adjunto.</p>" +
                                          "<p style = 'color: red; line-height: 0.1;'> Deberán remitirla por correo electrónico dentro de los 3 días de recibida esta solicitud, adjuntando archivo WORD o PDF que indique costos, plazos de entrega </p>" +
                                          "<p style = 'color: red; line-height: 0.1;'>y condiciones de contratación.</p>" +
                                          "<p style = 'line-height: 0.1;padding-left:50px;'> Deberán contemplar en todos los casos entrega en nuestro Centro de Distribución situado dentro del Mercado Central de Buenos Aires.</p>  " +
                                          "<p style ='line-height: 0.1;'><br></p>  " +
                                          "<p style ='line-height: 0.1;'>        Muchas Gracias.    </p>    " +
                                          "<p style ='line-height: 0.1;'> Dpto.Compras No productivas</p>" +
                                      "</div> ";

                        mailSender.Send("Diarco S.A - Pedido de Cotización", "sgm.oc.test.1@gmail.com", p.Proveedor.Email, body, "pedido_cotizacion.pdf", file, true);

                    });

                    requisicion.CotizacionPedida = true;
                    unitOfWork.RequisicionRepo.Update(requisicion);
                    unitOfWork.SaveChanges();
                    result.Data = true;
                    file = null;
                }

                return result;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error($"Ocurrio un error al generar la cotizacon para la requisicion {requisicionId}", ex);
                throw ex;
            }
        }

        public BusinessResult<bool> Approve(int requisicionId, int presupuestoId, List<RequisicionItemPrecio> precios, string comentarioAprobacion, int sucursalAEntregar)
        {
            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();
                BusinessResult<bool> result = new BusinessResult<bool>();

                Presupuesto presupuesto = unitOfWork.PresupuestoRepo.DbSet.FirstOrDefault(p => p.RequisicionId == requisicionId && p.Id == presupuestoId);
                if (presupuesto == null)
                {
                    result.Validation.Add("No existe el presupuesto");
                    return result;
                }

                if (!presupuesto.Cotizacion.HasValue || presupuesto.Aprobado)
                {
                    result.Validation.Add("Presupuesto sin cotización o aprobado");
                    return result;
                }

                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                if (!usuario.CanApproveByMonto(presupuesto.Cotizacion.Value))
                {
                    result.Validation.Add($"La cotización supera el monto de aprobación. Monto máximo posible ${usuario.GetMontoMaximo()} ");
                    return result;
                }

                Requisicion requisicion1 = unitOfWork.RequisicionRepo.GetRequisicionById(requisicionId);

                if (result.Validation.IsValid)
                {

                    requisicion1.Items.ForEach(ri =>
                    {
                        ri.Precio = precios.First(p => p.RequisicionItemId == ri.Id).Precio;
                        unitOfWork.RequisicionItemRepo.Update(ri);
                    });

                    presupuesto.UsuarioModificacionId = usuario.Id;
                    presupuesto.ComentarioAprobacion = comentarioAprobacion;
                    presupuesto.Modificacion = DateTime.Now;
                    presupuesto.Aprobado = true;
                    requisicion1.SucursalAEntregarId = sucursalAEntregar;
                    unitOfWork.RequisicionRepo.Update(requisicion1);
                    unitOfWork.PresupuestoRepo.Update(presupuesto);
                    unitOfWork.SaveChanges();

                    result.Data = true;
                }

                return result;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Error en la aprobacion del presupuesto", ex);
                throw ex;
            }
        }

        public BusinessResult<List<Presupuesto>> DeletePresupuesto(int requisicionId, int presupuestoId)
        {
            try
            {

                UnitOfWork unitOfWork = UnitOfWork.Create();
                BusinessResult<List<Presupuesto>> result = new BusinessResult<List<Presupuesto>>();
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                if (!usuario.IsInRole("aco"))
                {
                    result.Validation.Add("No tiene permisos para eliminar un prespuesto");
                    return result;
                }

                Requisicion requisicion = unitOfWork.RequisicionRepo.DbSet.FirstOrDefault(r => r.Id == requisicionId);

                if (requisicion == null || (requisicion != null && requisicion.EstadoId != RequisicionEstadosConstants.EnPresupuestacion))
                {
                    result.Validation.Add("No se puede eliminar el presupuesto");
                    return result;
                }

                List<Presupuesto> lsPresupuestos = unitOfWork.PresupuestoRepo.DbSet
                    .Where(p => p.RequisicionId == requisicionId)
                    .Include(p => p.Proveedor)
                    .ToList();

                var presupuesto = lsPresupuestos.FirstOrDefault(p => p.Id == presupuestoId);

                if (presupuesto == null || (presupuesto != null && presupuesto.Aprobado))
                {
                    result.Validation.Add("No se puede eliminar el presupuesto");
                    return result;
                }

                if (result.Validation.IsValid)
                {
                    lsPresupuestos.Remove(presupuesto);
                    unitOfWork.PresupuestoRepo.Delete(presupuesto);
                    unitOfWork.SaveChanges();
                    result.Data = lsPresupuestos;
                }

                return result;

            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Error eliminando el presupuesto", ex);
                throw ex;
            }
        }



    }
}
