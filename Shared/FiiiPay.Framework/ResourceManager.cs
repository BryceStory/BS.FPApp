using System;
using System.Globalization;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.ResourceManager
    /// </summary>
    public class ResourceManager
    {
        private readonly Type _type;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ResourceManager(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        public string GetResource(string resourceKey)
        {
            return this.GetResource(resourceKey, (CultureInfo)null);
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns></returns>
        public string GetResource(string resourceKey, string cultureName)
        {
            return GetResource(resourceKey, new CultureInfo(cultureName));
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">resourceKey</exception>
        public string GetResource(string resourceKey, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(resourceKey))
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }
            
            //var resource = new System.Resources.ResourceManager(_resourceName, Assembly.GetExecutingAssembly());
            var resource = new System.Resources.ResourceManager(_type);
            return resource.GetString(resourceKey, cultureInfo);
        }

        /// <summary>
        /// Gets the format resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string GetFormatResource(string resourceKey, params object[] args)
        {
            return this.GetFormatResource(resourceKey, (CultureInfo)null, args);
        }

        /// <summary>
        /// Gets the format resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="cultureName">Name of the culture.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string GetFormatResource(string resourceKey, string cultureName, params object[] args)
        {
            return this.GetFormatResource(resourceKey, new CultureInfo(cultureName), args);
        }

        /// <summary>
        /// Gets the format resource.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string GetFormatResource(string resourceKey, CultureInfo cultureInfo, params object[] args)
        {
            var resource = this.GetResource(resourceKey, cultureInfo);

            if (resource == null)
            {
                return null;
            }

            if (args.Length > 0)
            {
                return string.Format(resource, args);
            }

            return resource;
        }
    }
}