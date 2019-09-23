using System;
using System.Reflection;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// Class FiiiPay.Framework.Component.BaseComponent
    /// </summary>
    public abstract class BaseComponent
    {
        private readonly ILog _log;

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public Guid AccountId { get; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        /// <value>
        /// The name of the component.
        /// </value>
        public string ComponentName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponent"/> class.
        /// </summary>
        protected BaseComponent()
        {
            var type = GetType();
            _log = LogManager.GetLogger(type);

            ComponentName = type.FullName;
        }

        protected BaseComponent(Guid? accountId, string language = "en-US")
        {
            if (accountId.HasValue)
                AccountId = accountId.Value;

            Language = language;
        }
        
        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="index">The index.</param>
        protected virtual void Info(string message, string index = "")
        {
            if (!string.IsNullOrWhiteSpace(index))
            {
                _log.InfoFormat("component={0}, action={1}, info={2}, index={3}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    message, index);
            }
            else
            {
                _log.InfoFormat("component={0}, action={1}, info={2}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    message);
            }
        }

        /// <summary>
        /// Errors the specified code.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="index">The index.</param>
        protected virtual void Error(string errorMessage, string index = "")
        {
            if (!string.IsNullOrWhiteSpace(index))
            {
                _log.ErrorFormat("component={0}, action={1}, error={2}, index={3}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    errorMessage, index);
            }
            else
            {
                _log.ErrorFormat("component={0}, action={1}, error={2}", ComponentName, MethodBase.GetCurrentMethod().Name, errorMessage);
            }
        }

        /// <summary>
        /// Errors the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="index">The index.</param>
        protected virtual void Error(int code, string errorMessage, string index = "")
        {
            if (!string.IsNullOrWhiteSpace(index))
            {
                _log.ErrorFormat("component={0}, action={1}, code={2}, error={3}, index={4}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    code, errorMessage, index);
            }
            else
            {
                _log.ErrorFormat("component={0}, action={1}, code={2}, error={3}", ComponentName, MethodBase.GetCurrentMethod().Name, code,
                    errorMessage);
            }
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="index">The index.</param>
        protected virtual void Error(Exception exception, string index = "")
        {
            if (!string.IsNullOrWhiteSpace(index))
            {
                _log.ErrorFormat("component={0}, action={1}, error={2}, index={3}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    exception.Message, index);
            }
            else
            {
                _log.ErrorFormat("component={0}, action={1}, error={2}", ComponentName, MethodBase.GetCurrentMethod().Name, exception.Message);
            }
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="index">The index.</param>
        protected virtual void Error(CommonException exception, string index = "")
        {
            if (!string.IsNullOrWhiteSpace(index))
            {
                _log.ErrorFormat("component={0}, action={1}, code={2}, error={3}, index={4}", ComponentName, MethodBase.GetCurrentMethod().Name,
                    exception.ReasonCode, exception.Message, index);
            }
            else
            {
                _log.ErrorFormat("component={0}, action={1}, code={2}, error={3}", ComponentName, MethodBase.GetCurrentMethod().Name, exception.ReasonCode, exception.Message);
            }
        }
    }
}
