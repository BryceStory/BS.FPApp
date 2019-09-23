using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.RestUtilities
    /// </summary>
    public class RestUtilities
    {
        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public static string GetJson(string url, Dictionary<string, string> headers = null)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return HttpResponse(request);
        }

        /// <summary>
        /// Posts the json.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string PostJson(string url, string parameters)
        {
            return PostJson(url, null, parameters);
        }

        /// <summary>
        /// Posts the json asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static async Task<string> PostJsonAsync(string url, string parameters)
        {
            return await PostJsonAsync(url, null, parameters);
        }

        /// <summary>
        /// Posts the json.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string PostJson(string url, Dictionary<string, string> headers, string parameters)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var buffer = Encoding.UTF8.GetBytes(parameters);
            request.ContentLength = buffer.Length;
            using (var writer = request.GetRequestStream())
            {
                writer.Write(buffer, 0, buffer.Length);
            }
            return HttpResponse(request);
        }

        /// <summary>
        /// Posts the json asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static async Task<string> PostJsonAsync(string url, Dictionary<string, string> headers, string parameters)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var buffer = Encoding.UTF8.GetBytes(parameters);
            request.ContentLength = buffer.Length;
            using (var writer = request.GetRequestStream())
            {
                writer.Write(buffer, 0, buffer.Length);
            }
            return await HttpResponseAsync(request);
        }

        /// <summary>
        /// Posts the json.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        public static string PostJson(string url, Dictionary<string, string> headers, string parameters, out HttpStatusCode statusCode)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var bytes = Encoding.UTF8.GetBytes(parameters);
            request.ContentLength = bytes.Length;
            using (var writer = request.GetRequestStream())
            {
                writer.Write(bytes, 0, bytes.Length);
            }
            return HttpResponse(request, out statusCode);
        }
        
        private static string HttpResponse(WebRequest request)
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }

            if (response == null)
                throw new NullReferenceException(request.RequestUri.ToString());

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var stream = response.GetResponseStream();
            if (stream == null)
            {
                return null;
            }
            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private static async Task<string> HttpResponseAsync(WebRequest request)
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var stream = response.GetResponseStream();
            if (stream == null)
            {
                return null;
            }
            string result;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }

        private static string HttpResponse(WebRequest request, out HttpStatusCode statusCode)
        {
            if (request.GetResponse() is HttpWebResponse response)
            {
                statusCode = response.StatusCode;

                var stream = response.GetResponseStream();
                if (stream == null)
                {
                    return null;
                }

                string result;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                return result;
            }

            statusCode = HttpStatusCode.BadRequest;
            return string.Empty;
        }
    }
}
