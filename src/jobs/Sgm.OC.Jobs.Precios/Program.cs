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
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using Sgm.OC.Jobs.Precios.Entities;

namespace Sgm.OC.Jobs.Precios
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

                Serilogger.Logger.Information("Job - Precios");
                Serilogger.Logger.Information("Configurando conexión con base de datos");

                Sgm.OC.Framework.Settings.ConnectionString = System.Configuration.ConfigurationManager.AppSettings.Get("conn");

                UnitOfWork unitOfWork = UnitOfWork.Create();

                Serilogger.Logger.Information("Obteniendo usuario de Sistema");
                LoginResult loginResult = new UsuarioBusiness().Login("sistema", 1);

                if (!loginResult.IsOk)
                    throw new SecurityException("No autorizado para ejecutar el proceso");

                RequisicionBussiness requisicionBussiness = new RequisicionBussiness();
                requisicionBussiness.UserLogged = new UserLogged(loginResult.LoginName, loginResult.Roles, 1);

                Serilogger.Logger.Information("Buscando productos");

                List<Producto> productos =
                    unitOfWork.ProductoRepo.DbSet.ToList();
                Serilogger.Logger.Information($"Se encontraron { productos.Count} productos");

                SqlConnection conexion = new SqlConnection(ConfigurationManager.AppSettings.Get("connDiarco"));

                int skipAcum = 0;
                int count = 0;

                var prodsIds = productos.Select(p => p.IdInterno.ToString()).ToList();

                while (skipAcum <= productos.Count())
                {
                    UnitOfWork unitOfWorkProds = UnitOfWork.Create();

                    conexion.Open();
                    List<ItemsSGM> itemsSgm = new List<ItemsSGM>();

                    var prodsIdsSelected = prodsIds.Skip(skipAcum).Take(50);

                    if (prodsIdsSelected.Any())
                    {
                        string inStr = string.Join(",", prodsIdsSelected);
                        string instruccion = $"select I_PRECIO_VTA,Q_FACTOR_VTA_SUCU,C_ARTICULO from T051_ARTICULOS_SUCURSAL where C_SUCU_EMPR = 1 and C_ARTICULO in ({inStr})";
                        SqlCommand comm = new SqlCommand(instruccion, conexion);
                        SqlDataReader dr = comm.ExecuteReader();
                        while (dr.Read())
                        {
                            ItemsSGM itemSgm = new ItemsSGM();
                            itemSgm.Precio = decimal.Parse(dr[0].ToString());
                            itemSgm.Factor = int.Parse(dr[1].ToString());
                            itemSgm.IdInterno = int.Parse(dr[2].ToString());
                            count++;
                            itemsSgm.Add(itemSgm);
                        }
                        dr.Close();

                        skipAcum += 50;
                    }

                    foreach (var item in itemsSgm)
                    {
                        var prod = productos.FirstOrDefault(p => p.IdInterno == item.IdInterno);
                        prod.Precio = item.Precio;
                        prod.FactorConversion = item.Factor;
                        unitOfWorkProds.ProductoRepo.Update(prod);
                    }
                    conexion.Close();
                    unitOfWorkProds.SaveChanges();
                }

                Serilogger.Logger.Information($"Fin del proceso. Precios actualizados: {count}");

            }
            catch (Exception ex)
            {
                Serilogger.Logger.Error(ex, "Error ejecutando proceso");
            }

        }
    }
}
