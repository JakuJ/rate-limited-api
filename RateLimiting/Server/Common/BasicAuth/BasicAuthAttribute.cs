using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Common.BasicAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter)) => Arguments = Array.Empty<object>();
    }
}