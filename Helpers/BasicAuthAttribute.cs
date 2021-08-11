using System;
using Microsoft.AspNetCore.Mvc;

namespace RateLimiting.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute(string? realm = null) : base(typeof(BasicAuthFilter))
            => Arguments = new object?[] {realm};
    }
}