using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Common.BasicAuth
{
    /// <summary>Adds basic authorization to an endpoint.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="BasicAuthAttribute"/> class.</summary>
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter)) => Arguments = Array.Empty<object>();
    }
}
