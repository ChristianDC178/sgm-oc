using System;
using System.Net.Sockets;
using Microsoft.AspNetCore.Components;
using Serilog;
using Serilog.Sinks.Udp.TextFormatters;
using Serilogger = Serilog.Log;

namespace Sgm.OC.Log
{

    public class NullLoggerImplementation : ILogger
    {
        public void Debug(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public void Error(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public void Fatal(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public void Flush()
        {
        }

        public void Info(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.Write(message);
        }

        public void Warn(string message, Exception ex = null)
        {
            System.Diagnostics.Debug.Write(message);
        }
    }

    public class SerilogImplementation : ILogger
    {

        public SerilogImplementation()
        {
            Serilogger.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .WriteTo.Udp(
                      "localhost",
                      7071,
                      AddressFamily.InterNetwork,
                      new Log4jTextFormatter())
                  .WriteTo.Debug()
                  .WriteTo.Console()
                  .WriteTo.File("logs/log.txt", Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                  .CreateLogger();
        }

        public void Debug(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Serilogger.Logger.Debug(ex, message);
            }
            else
            {
                Serilogger.Logger.Debug(message);
            }
        }

        public void Info(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Serilogger.Logger.Information(ex, message);
            }
            else
            {
                Serilogger.Logger.Information(message);
            }
        }

        public void Warn(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Serilogger.Logger.Warning(ex, message);
            }
            else
            {
                Serilogger.Logger.Warning(message);
            }
        }

        public void Error(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Serilogger.Logger.Error(ex, message);
            }
            else
            {
                Serilogger.Logger.Error(message);
            }
        }

        public void Fatal(string message, Exception ex = null)
        {
            if (ex != null)
            {
                Serilogger.Logger.Fatal(ex, message);
            }
            else
            {
                Serilogger.Logger.Fatal(message);
            }
        }

        public void Flush()
        {
            Serilogger.CloseAndFlush();
        }

    }

}
