using System.Net;
using System.Net.Http.Headers;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// Class FiiiPay.Data.Agents.JPush.Model.HttpResponse
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public HttpResponseHeaders Headers { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="content">The content.</param>
        public HttpResponse(HttpStatusCode statusCode, HttpResponseHeaders headers, string content)
        {
            StatusCode = statusCode;
            Headers = headers;
            Content = content;
        }
    }
}
