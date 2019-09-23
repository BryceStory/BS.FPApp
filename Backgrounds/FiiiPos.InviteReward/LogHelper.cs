using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPos.InviteReward
{
    public class LogHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger("FiiiPay");

        public static void Config()
        {
            XmlConfigurator.Configure();
        }

        public static void Info(object message, Exception exception = null)
        {
            if (exception == null)
            {
                if (message is Exception)
                {
                    var _exception = (Exception)message;
                    _logger.Info(_exception.Message, _exception);
                }
                else
                {
                    _logger.Info(message);
                }
            }
            else
            {
                _logger.Info(message, exception);
            }
        }

        public static void Debug(object message, Exception exception = null)
        {
            if (exception == null)
            {
                if (message is Exception exception1)
                {
                    _logger.Debug(exception1.Message, exception1);
                }
                else
                {
                    _logger.Debug(message);
                }
            }
            else
            {
                _logger.Debug(message, exception);
            }
        }

        public static void Warn(object message, Exception exception = null)
        {
            if (exception == null)
            {
                if (message is Exception)
                {
                    var _exception = (Exception)message;
                    _logger.Warn(_exception.Message, _exception);
                }
                else
                {
                    _logger.Warn(message);
                }
            }
            else
            {
                _logger.Warn(message, exception);
            }
        }

        public static void Error(object message, Exception exception = null)
        {
            if (exception == null)
            {
                if (message is Exception)
                {
                    var _exception = (Exception)message;
                    _logger.Error(_exception.Message, _exception);
                }
                else
                {
                    _logger.Error(message);
                }
            }
            else
            {
                _logger.Error(message, exception);
            }
        }

        public static void Fatal(object message, Exception exception = null)
        {
            if (exception == null)
            {
                if (message is Exception)
                {
                    var _exception = (Exception)message;
                    _logger.Fatal(_exception.Message, _exception);
                }
                else
                {
                    _logger.Fatal(message);
                }
            }
            else
            {
                _logger.Fatal(message, exception);
            }
        }

        public static void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }
    }
}
