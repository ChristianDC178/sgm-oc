using System;
using System.Collections.Generic;
using System.Linq;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;
using Sgm.OC.Repository;
using Sgm.OC.Security;
using Sgm.OC.Security.Entities;
using Serilog;
using Serilogger = Serilog.Log;
using System.Security;
using Sgm.OC.Framework;

namespace Sgm.OC.Jobs.ConsolidacionAutomatica
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

                Serilogger.Logger.Information("Job - Consolidación Automática Iniciando");
                Serilogger.Logger.Information("Configurando conexión con base de datos");

                Sgm.OC.Framework.Settings.ConnectionString = System.Configuration.ConfigurationManager.AppSettings.Get("conn");

                Serilogger.Logger.Information("Obteniendo usuario de Sistema");
                LoginResult loginResult = new UsuarioBusiness().Login("sistema", 1);

                if (!loginResult.IsOk)
                    throw new SecurityException("No autorizado para ejecutar el proceso");

                JobsUnitOfWork jobsUoW = JobsUnitOfWork.Create();
                UnitOfWork unitOfWork = UnitOfWork.Create();

                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                requisicionBussiness.UserLogged = new UserLogged(loginResult.LoginName, loginResult.Roles, 1);

                Serilogger.Logger.Information("Buscando Pedidos para consolidar");

                List<ConsolidacionPendiente> list = jobsUoW.RequisicionAutoRepo.DbSet.ToList();
                var rubros = list.Select(i => i.RubroIdInterno).Distinct();

                Serilogger.Logger.Information($"Pedidos encontrados {list.Count}");
                Serilogger.Logger.Information("Recorriendo rubros para consolidar");

                foreach (var a in list)
                {
                    Serilogger.Logger.Information(a.ToString());
                }

                int countRecurrentes = 0;
                int countNoRecurrentes = 0;

                foreach (var item in rubros)
                {
                    var itemsRecurrentes = list.Where(i => i.RubroIdInterno == item && i.Recurrente).ToList();
                    var itemsNoRecurrentes = list.Where(i => i.RubroIdInterno == item && !(i.Recurrente)).ToList();

                    var pedidoItemsIdsRecurrentes = itemsRecurrentes.Select(i => i.PedidoItemId).ToArray();
                    var pedidoItemsIdsNoRecurrentes = itemsNoRecurrentes.Select(i => i.PedidoItemId).ToArray();

                    List<PedidoItem> pedidoItemsRecurrentes = unitOfWork.PedidoItemRepo.DbSet.Where(pi => pedidoItemsIdsRecurrentes.Contains(pi.Id)).ToList();
                    List<PedidoItem> pedidoItemsNoRecurrentes = unitOfWork.PedidoItemRepo.DbSet.Where(pi => pedidoItemsIdsNoRecurrentes.Contains(pi.Id)).ToList();

                    var piGroupedRecurrentes = pedidoItemsRecurrentes.GroupBy(pi => pi.ProductoId);
                    var piGroupedNoRecurrentes = pedidoItemsNoRecurrentes.GroupBy(pi => pi.ProductoId);

                    RequisicionRequest requisicionRequestRecurrente = new RequisicionRequest() { Recurrente = true };
                    RequisicionRequest requisicionRequestNoRecurrente = new RequisicionRequest() { Recurrente = false };

                    foreach (var grouped in piGroupedRecurrentes)
                    {
                        requisicionRequestRecurrente.Items.Add(new RequisicionItemRequest()
                        {
                            ProductoId = grouped.Key,
                            Cantidad = grouped.Sum(g => g.Cantidad),
                            PedidoItemIds = pedidoItemsRecurrentes.Select(pi => pi.Id).ToArray(),
                            EnUnidades = false,
                        }); ;
                    }

                    foreach (var grouped in piGroupedNoRecurrentes)
                    {
                        requisicionRequestNoRecurrente.Items.Add(new RequisicionItemRequest()
                        {
                            ProductoId = grouped.Key,
                            Cantidad = grouped.Sum(g => g.Cantidad),
                            PedidoItemIds = pedidoItemsRecurrentes.Select(pi => pi.Id).ToArray(),
                            EnUnidades = false
                        }); ;
                    }

                    Serilogger.Logger.Information("Creando nueva requisicion");

                    if(requisicionRequestRecurrente.Items.Count >= 1)
                    {
                        BusinessResult<int> requisicionIdRecurrente = requisicionBussiness.CreateRequisicion(requisicionRequestRecurrente);
                        Serilogger.Logger.Information($"Requisicion {requisicionIdRecurrente} creada");
                        countRecurrentes = requisicionRequestRecurrente.Items.Count;
                    }

                    if (requisicionRequestRecurrente.Items.Count >= 1)
                    {
                        BusinessResult<int> requisicionIdNoRecurrente = requisicionBussiness.CreateRequisicion(requisicionRequestNoRecurrente);
                        Serilogger.Logger.Information($"Requisicion {requisicionIdNoRecurrente} creada");
                        countNoRecurrentes = requisicionRequestNoRecurrente.Items.Count;
                    }
                }

                Serilogger.Logger.Information($"Fin del proceso. Se crearon {countRecurrentes} requisiciones recurrentes");
                Serilogger.Logger.Information($"Fin del proceso. Se crearon {countNoRecurrentes} requisiciones no recurrentes");
            }
            catch (Exception ex)
            {
                Serilogger.Logger.Error(ex, "Error ejecutando proceso");
            }
        }

    }
}
