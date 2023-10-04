using Serilog;
using ILogger = Serilog.ILogger;

namespace NorthWind.Helpers
{
    public class LoggerManager:ILoggerManager
    {
        private static readonly ILogger _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
    }
}
