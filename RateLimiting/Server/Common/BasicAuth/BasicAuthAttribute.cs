using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Common.BasicAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute(string? realm = null) : base(typeof(BasicAuthFilter))
            => Arguments = realm == null ? Array.Empty<object>() : new object[] {realm};
    }
}