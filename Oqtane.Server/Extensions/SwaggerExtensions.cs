using Microsoft.AspNetCore.Builder;
using Oqtane.Security;

namespace Oqtane.Extensions
{
    public static class SwaggerExtensions
    {
        public static IApplicationBuilder UseSwaggerSecurity(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SwaggerSecurityMiddleware>();
        }
    }
}