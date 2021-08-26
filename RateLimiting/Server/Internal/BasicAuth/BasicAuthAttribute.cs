using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Internal.BasicAuth
{
    /// <summary>Adds basic authorization to an endpoint.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class BasicAuthAttribute : TypeFilterAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="BasicAuthAttribute"/> class.</summary>
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter)) => Arguments = Array.Empty<object>();
    }
}
