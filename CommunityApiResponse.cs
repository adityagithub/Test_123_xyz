using Community.Services.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Community.Core
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommunityApiResponse<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityApiResponse{T}"/> class.
        /// </summary>
        /// <param name="data">Data (parsed HTTP body)</param>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="messages">Api messages.</param>
        public CommunityApiResponse(T data, int statusCode = StatusCodes.Status200OK, List<string> messages = null)
        {
            this.Data = data;
            this.StatusCode = statusCode;
            this.Messages = messages;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get { return CommunityApp.GetCurrentVersion(); } }

        /// <summary>
        /// Gets or sets the status code (HTTP status code)
        /// </summary>
        /// <value>The status code.</value>
        public int StatusCode { get; }

        /// <summary>
        /// Detail error message
        /// </summary>
        /// <value>HTTP headers</value>
        public List<string> Messages { get; }

        /// <summary>
        /// Gets or sets the data (parsed HTTP body)
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; }

        /// <summary>
        /// JSON string presentation of the object
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
