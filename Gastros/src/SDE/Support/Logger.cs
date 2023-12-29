using System;
using System.IO;
using log4net;
using log4net.Config;

namespace GastrOs.Sde.Support
{
    public static class Logger
    {
        public const string LogFile = "logging.xml";
        private static ILog log = LogManager.GetLogger(typeof(Logger));

        static Logger()
        {
            try
            {
                XmlConfigurator.Configure(new FileInfo(LogFile));
            } catch (Exception e)
            {
                BasicConfigurator.Configure();
            }
        }

        public static void MessageFormat(string message, params object[] formatParams)
        {
            log.InfoFormat(message, formatParams);
        }

        public static void Message(string message)
        {
            log.Info(message);
        }

        public static void ErrorFormat(string message, params object[] formatParams)
        {
            log.ErrorFormat(message, formatParams);
        }

        public static void Error(string message, Exception e)
        {
            log.Error(message, e);
        }

        public static void DebugFormat(string message, params object[] formatParams)
        {
            log.DebugFormat(message, formatParams);
        }
    }
}