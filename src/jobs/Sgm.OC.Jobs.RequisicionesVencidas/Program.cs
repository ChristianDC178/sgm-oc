using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Serilog;
using Sgm.OC.Core.Bussiness;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;
using Sgm.OC.Repositories;
using Sgm.OC.Security.Entities;
using Serilogger = Serilog.Log;

namespace Sgm.OC.Jobs.RequisicionesVencidas
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

                Serilogger.Logger.Information("Job - Requisiciones Vencidas Iniciado");
                Serilogger.Logger.Information("Configurando conexión con base de datos");

                Sgm.OC.Framework.Settings.ConnectionString = System.Configuration.ConfigurationManager.AppSettings.Get("conn");

                UnitOfWork unitOfWork = UnitOfWork.Create();

                UsuarioBusiness usuarioBusiness = new UsuarioBusiness();

                Serilogger.Logger.Information("Obteniendo usuario de Sistema");
                LoginResult loginResult = usuarioBusiness.Login("sistema", 1);

                if (!loginResult.IsOk)
                    throw new SecurityException("No autorizado para ejecutar el proceso");

                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                requisicionBussiness.UserLogged = new UserLogged(loginResult.LoginName, loginResult.Roles, 1);

                Serilogger.Logger.Information("Buscando requisiciones sin presupuesto");

                List<Requisicion> reqsSinPresupuestos =
                unitOfWork.RequisicionRepo.DbSet
                .Where(req => req.Presupuestos.Count() == 0 && req.EstadoId != RequisicionEstadosConstants.Rechazado)
                .ToList();

                Serilogger.Logger.Information($"Se encontraron {reqsSinPresupuestos.Count} requisiones");

                int count = 0;

                foreach (var item in reqsSinPresupuestos)
                {
                    if (DateTime.Now.Subtract(item.Creacion).Days >= 30)
                    {
                        BusinessResult<ChangeWorkflowEstadoResult> result = requisicionBussiness.ChangeEstado(item.Id, "Rechazada automáticamente por no contener presupuestos y pasar 30 dias desde su creación", true);
                        if (result.Validation.IsValid)
                        {
                            Serilogger.Logger.Information($"Requisiion {item.Id} rechazada correctamente");
                            count++;
                        }
                        else
                        {
                            Serilogger.Logger.Information($"Requisiion {item.Id} no se pudo rechazar correctamente");
                            result.Validation.Items.ForEach(i =>
                            {
                                Serilogger.Logger.Information($"Motivo: {i.Message}");
                            });
                        }
                    }
                }

                Serilogger.Logger.Information($"Fin del proceso. Requisiciones rechazadas: {count}");

            }
            catch (Exception ex)
            {
                Serilogger.Logger.Error(ex, "Error ejecutando proceso");
            }
        }
    }
}
