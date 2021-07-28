using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Sgm.OC.Log;

namespace Sgm.OC.Framework
{

    public class SgmApplication
    {

        public static Sgm.OC.Log.ILogger Logger { get; set; }

        public void Start<TStartup>() where TStartup : class
        {
            try
            {
                WebHost.CreateDefaultBuilder()
                    .UseSerilogImplementation()
                    .UseIISIntegration()
                    .UseStartup<TStartup>()
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Fatal("The application ends anormally", ex);
            }
            finally
            {
                SgmApplication.Logger.Flush();
            }
        }

        public void StartOfflineApplication()
        {
            try
            {
                Sgm.OC.Framework.SgmApplication.Logger = new NullLoggerImplementation();
            }
            catch (Exception ex)
            {
                SgmApplication.Logger.Fatal("The application ends anormally", ex);
            }
            finally
            {
                SgmApplication.Logger.Flush();
            }
        }
    }

    public static class SgmWebHostBuilderExtensions
    {

        public static IWebHostBuilder UseSerilogImplementation(this IWebHostBuilder builder)
        {
           
            Sgm.OC.Framework.SgmApplication.Logger = new SerilogImplementation();

            builder.UseSerilog();

            return builder;
        }

    }


}
