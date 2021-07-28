using System;
using System.Collections.Generic;
using System.Linq;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;
using Sgm.OC.Security.Entities;
using Serilog;
using Serilogger = Serilog.Log;
using System.Security;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Jobs.EmisionOrdenCompra.Entities;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Engines;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Sgm.OC.Jobs.EmisionOrdenCompra
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                //Bug de Serilog cuando se hace encapsulacion
                Serilogger.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                Sgm.OC.Framework.SgmApplication sgmApplication = new Framework.SgmApplication();
                sgmApplication.StartOfflineApplication();

                Serilogger.Logger.Information("Job - Emision Orden de Compra");
                Serilogger.Logger.Information("Configurando conexión con base de datos");

                Sgm.OC.Framework.Settings.ConnectionString = System.Configuration.ConfigurationManager.AppSettings.Get("conn");

                UnitOfWork unitOfWork = UnitOfWork.Create();

                Serilogger.Logger.Information("Obteniendo usuario de Sistema");
                LoginResult loginResult = new UsuarioBusiness().Login("sistema", 1);

                if (!loginResult.IsOk)
                    throw new SecurityException("No autorizado para ejecutar el proceso");

                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                requisicionBussiness.UserLogged = new UserLogged(loginResult.LoginName, loginResult.Roles, 1);

                Serilogger.Logger.Information("Buscando requisiciones en estado Emitido");
                List<Requisicion> requisiciones =
                    unitOfWork.RequisicionRepo.DbSet
                    .Where(req => req.EstadoId == RequisicionEstadosConstants.Emitido && req.OrdenCompraKey.Sufijo == null)
                    .Include(i => i.Items)
                        .ThenInclude(i => i.Producto)
                    .Include(i => i.Usuario)
                    .ToList();

                Serilogger.Logger.Information($"Se encontraron { requisiciones.Count} requisiones");

                int count = 0;

                SqlConnection conexion = new SqlConnection(ConfigurationManager.AppSettings.Get("connDiarco"));
                conexion.Open();
                SqlCommand commStartingPoint = new SqlCommand("SELECT TOP 1 U_SUFIJO_OC FROM T080_OC_CABE ORDER BY F_ALTA_SIST DESC", conexion);
                SqlDataReader readerStartingPoint = commStartingPoint.ExecuteReader();

                int startingPointSuffix = 0;

                while (readerStartingPoint.Read())
                {
                    startingPointSuffix = (int) readerStartingPoint.GetDecimal(0);
                }

                readerStartingPoint.Close();
                conexion.Close();


                foreach (var item in requisiciones)
                {
                    startingPointSuffix++;
                    item.OrdenCompraKey.Sufijo = startingPointSuffix;
                    OCCabecera cabecera = new OCCabecera();
                    //ver como hago aca cabecera.Sufijo ;
                    //Analisis tabla de proveedores ??? cabecera.DiasLimiteEntrega
                    cabecera.Sufijo = startingPointSuffix;
                    cabecera.IdProveedor = 1;//item.Presupuestos.FirstOrDefault(p => p.Aprobado).Proveedor.IdInterno;
                    cabecera.FechaSituacion = DateTime.Now;
                    cabecera.FechaAlta = DateTime.Now;
                    cabecera.FechaEmision = DateTime.Now;
                    cabecera.ImporteNeto = item.Items.Sum(i => i.Precio ?? 0);
                    cabecera.ImporteIVA = cabecera.ImporteNeto;//TEMPORAL
                    cabecera.ImporteImpuestoInt = cabecera.ImporteNeto;//TEMPORAL
                    cabecera.ImporteTotal = cabecera.ImporteNeto;//TEMPORAL
                    cabecera.CompradorId = 0;//TEMPORAL
                    cabecera.OperadorId = item.UsuarioId;//TEMPORAL
                    cabecera.TerminalOperador = "TR106";//TEMPORAL
                    cabecera.UsuarioCumplido = item.Usuario.Login;
                    cabecera.PlazoEntrega1 = 30; //TEMPORAL
                    cabecera.CondicionPago = 30; //TEMPORAL SALE DE ARTICULO, TABLA DE ARTICULO
                    cabecera.Observacion = item.ComentarioAprobacion;
                    cabecera.UsuarioModifId = item.Usuario.Login;
                    cabecera.TerminalModifId = cabecera.TerminalOperador;
                    cabecera.FechaModif = DateTime.Now;
                    //cabecera.TipoProveedor = 1; //TEMPORAL SACAR DE TABLA PROVEEDOR POR IDINTERNO
                    cabecera.IdSucursal = item.SucursalId;
                    cabecera.IdSucursalDestino = item.SucursalAEntregarId ?? item.SucursalId;
                    cabecera.IdSucursalDestinoAlt = item.SucursalAEntregarId ?? item.SucursalId;
                    foreach (var reqitem in item.Items)
                    {
                        OCDetalle detalle = new OCDetalle();
                        detalle.Prefijo = cabecera.Prefijo;
                        detalle.Sufijo = cabecera.Sufijo;
                        detalle.IdInterno = reqitem.Producto.IdInterno;
                        detalle.CantBultosSugerido = reqitem.Cantidad;
                        detalle.CantBultosProveedor = reqitem.Cantidad * reqitem.Producto.FactorConversion;
                        detalle.CantFactorProveedor = reqitem.Producto.FactorConversion;
                        detalle.CantBultosEmpr = reqitem.Cantidad * reqitem.Producto.FactorConversion;
                        detalle.CantFactorProveedor = reqitem.Producto.FactorConversion;
                        detalle.CantBultosEmpr = reqitem.Producto.FactorConversion;
                        detalle.CantFactorEmpr = 1; //TEMPORAL
                        detalle.PesoUnit = 0;//SACAR PESOUNIT DE T050_ARTICULOS TEMPORAL
                        detalle.PesoTotal = 0; //SUMA DE TODOS LOS PESOS, TEMPORAL
                        detalle.IvaCalculo = 2; //TEMPORAL, SALE DE T055_....
                        detalle.CoeficienteIva = 0; //TEMPORAL, SALE DE T051
                        detalle.KImpuestoInterno = 0; //TEMPORAL, SALE DE T051
                        detalle.CostoBase = reqitem.Precio ?? 0; //TEMPORAL, SALE DE T055
                        detalle.PrecioCompra = reqitem.Precio ?? 0; //TEMPORAL, NO HAY DEFINICION
                        //PRECIO PARTE Y PRECIOLISTA PREGUNTAR A HERNAN POR MAIL
                        detalle.ImpuestoInterno = 0; //TEMPORAL, SALE DE T055
                        detalle.TotalImpuestoInterno = 0; //TEMPORAL, VER CALCULO
                        detalle.TotalItem = detalle.TotalImpuestoInterno + detalle.PrecioLista;
                        detalle.CodigoDescuentoComp1 = 105; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp1 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp2 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp2 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp3 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp3 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp4 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp4 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp5 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp5 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp6 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp6 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp7 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp7 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp8 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp8 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp9 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp9 = 0; //TEMPORAL SALE DE T055
                        detalle.CodigoDescuentoComp10 = 0; //TEMPORAL, SALE DE T055
                        detalle.DescuentoComp10 = 0; //TEMPORAL SALE DE T055

                        cabecera.Detalles.Add(detalle);

                    }

                    //aca hago el insert en BD
                    count++;
                    // CAMPOS TipoProveedor y Especial
                    //string instruccion = string.Format("INSERT INTO T080_OC_CABE VALUES ({0},{1},{2},{3},{4},'{5}','{6}','{7}',{8},{9},{10},{11},{12},{13},'{14}','{15}','{16}','{17}',{18},{19},{20},{21},{22},'{23}','{24}','{25}','{26}',{27},{28},{29},{30},{31},{32},'{33}','{34}','{35}',{36},{37},{38},'{39}',{40},'{41}','{42}','{43}','{44}',{45},'{46}','{47}','{48}',{49})", cabecera.Oc, cabecera.Prefijo, cabecera.Sufijo, cabecera.PrefijoLote, cabecera.Lote, cabecera.OcMadre, cabecera.OcTransferencia, cabecera.OcPagoAnterior, cabecera.DiasLimiteEntrega, cabecera.IdProveedor, cabecera.IdSucursal, cabecera.IdSucursalDestino, cabecera.IdSucursalDestinoAlt, cabecera.Situacion, cabecera.FechaSituacion, cabecera.FechaAlta, cabecera.FechaEmision, cabecera.FechaEntrega, cabecera.ImporteNeto, cabecera.ImporteIVA, cabecera.ImporteImpuestoInt, cabecera.ImporteTotal, cabecera.CompradorId, cabecera.OperadorId, cabecera.TerminalOperador, cabecera.UsuarioCumplido, cabecera.Tipo, cabecera.PlazoEntrega1, cabecera.PlazoEntrega2, cabecera.PlazoEntrega3, cabecera.PlazoEntrega4, cabecera.PlazoEntrega5, cabecera.PlazoEntrega6, cabecera.CondicionPago, cabecera.Observacion, cabecera.FechaIng, cabecera.CodigoIng, cabecera.PrefijoCodigoIng, cabecera.SufijoCodigoIng, cabecera.ObservacionIng, cabecera.TipoEntrega, cabecera.UsuarioModifId, cabecera.TerminalModifId, cabecera.FechaModif, cabecera.OcElectronica, cabecera.SituacOc, cabecera.FechaSituacOc, cabecera.Enviado, cabecera.Especial, cabecera.TipoProveedor);
                    // SIN CAMPOS TipoProveedor y Especial
                    string instruccion = string.Format("INSERT INTO T080_OC_CABE VALUES ({0},{1},{2},{3},{4},'{5}','{6}','{7}',{8},{9},{10},{11},{12},{13},'{14}','{15}','{16}','{17}',{18},{19},{20},{21},{22},'{23}','{24}','{25}','{26}',{27},{28},{29},{30},{31},{32},'{33}','{34}','{35}',{36},{37},{38},'{39}',{40},'{41}','{42}','{43}','{44}',{45},'{46}','{47}')", cabecera.Oc, cabecera.Prefijo, cabecera.Sufijo, cabecera.PrefijoLote, cabecera.Lote, cabecera.OcMadre, cabecera.OcTransferencia, cabecera.OcPagoAnterior, cabecera.DiasLimiteEntrega, cabecera.IdProveedor, cabecera.IdSucursal, cabecera.IdSucursalDestino, cabecera.IdSucursalDestinoAlt, cabecera.Situacion, cabecera.FechaSituacion, cabecera.FechaAlta, cabecera.FechaEmision, cabecera.FechaEntrega, cabecera.ImporteNeto, cabecera.ImporteIVA, cabecera.ImporteImpuestoInt, cabecera.ImporteTotal, cabecera.CompradorId, cabecera.OperadorId, cabecera.TerminalOperador, cabecera.UsuarioCumplido, cabecera.Tipo, cabecera.PlazoEntrega1, cabecera.PlazoEntrega2, cabecera.PlazoEntrega3, cabecera.PlazoEntrega4, cabecera.PlazoEntrega5, cabecera.PlazoEntrega6, cabecera.CondicionPago, cabecera.Observacion, cabecera.FechaIng, cabecera.CodigoIng, cabecera.PrefijoCodigoIng, cabecera.SufijoCodigoIng, cabecera.ObservacionIng, cabecera.TipoEntrega, cabecera.UsuarioModifId, cabecera.TerminalModifId, cabecera.FechaModif, cabecera.OcElectronica, cabecera.SituacOc, cabecera.FechaSituacOc, cabecera.Enviado);

                    foreach (var deta in cabecera.Detalles)
                    {
                        instruccion = instruccion + string.Format(" INSERT INTO T081_OC_DETA VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},'{26}','{27}','{28}',{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50})", deta.Oc, deta.Prefijo, deta.Sufijo, deta.IdInterno, deta.CantBultosSugerido, deta.CantBultosProveedor, deta.CantFactorProveedor, deta.CantBultosBonificados, deta.CantBultosEmpr, deta.CantFactorEmpr, deta.PesoUnit, deta.PesoTotal, deta.PesoTotalBonif, deta.IvaCalculo, deta.CoeficienteIva, deta.KImpuestoInterno, deta.CostoBase, deta.PrecioCompra, deta.PrecioParte, deta.PrecioLista, deta.ImpuestoInterno, deta.Envases, deta.TotalImpuestoInterno, deta.TotalItem, deta.UnidadesCumplidas, deta.PesoCumplido, deta.CumplidoParcial, deta.UsuarioCumplidoParcial, deta.FechaCumplidoParcial, deta.PisoPaletizado, deta.AlturaPisoPaletizado, deta.CodigoDescuentoComp1, deta.DescuentoComp1, deta.CodigoDescuentoComp2, deta.DescuentoComp2, deta.CodigoDescuentoComp3, deta.DescuentoComp3, deta.CodigoDescuentoComp4, deta.DescuentoComp4, deta.CodigoDescuentoComp5, deta.DescuentoComp5, deta.CodigoDescuentoComp6, deta.DescuentoComp6, deta.CodigoDescuentoComp7, deta.DescuentoComp7, deta.CodigoDescuentoComp8, deta.DescuentoComp8, deta.CodigoDescuentoComp9, deta.DescuentoComp9, deta.CodigoDescuentoComp10, deta.DescuentoComp10);

                    }
                    conexion.Open();
                    SqlCommand comm = new SqlCommand(instruccion, conexion);
                    comm.ExecuteNonQuery();
                    unitOfWork.Context.Database.ExecuteSqlInterpolated($"UPDATE [Requisicion] SET [Sufijo] = {item.OrdenCompraKey.Sufijo} WHERE [RequisicionId] = {item.Id}");
                    Serilogger.Logger.Information($"Se grabo la Requisicion N.{item.Id} con el sufijo N.{item.OrdenCompraKey.Sufijo}");
                    conexion.Close();



                }

                Serilogger.Logger.Information($"Fin del proceso. Ordenes de compra emitidas: {count}");

            }
            catch (Exception ex)
            {
                Serilogger.Logger.Error(ex, "Error ejecutando proceso");
            }

        }
    }
}
