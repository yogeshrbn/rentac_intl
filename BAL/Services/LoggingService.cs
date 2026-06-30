using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class LoggingService
    {
        ILogger logger = LogManager.GetCurrentClassLogger();


        public void LogError(Exception ex, string message, params string[] strings)
        {
            logger.Error(ex, message, strings);
        }
        public void LogError(string message, params string[] strings)
        {
            logger.Error(message, strings);
        }
        public void LogInfo(Exception ex, string message, params string[] strings)
        {
            logger.Info(ex, message, strings);
        }
        public void LogInfo(string message, params string[] strings)
        {
            logger.Info(message, strings);
        }
        public void LogDebug(Exception ex, string message, params string[] strings)
        {
            logger.Debug(ex, message, strings);
        }
        public void LogDebug(string message, params string[] strings)
        {
            logger.Debug(message, strings);
        }
    }
}
