using Microsoft.AspNetCore.Http;

namespace LMS.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prevent MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Prevent clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // XSS Protection
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Content Security Policy
            // Start strict, but allow self-hosted scripts/styles
            // Skip CSP for Swagger/OpenAPI paths to prevent breaking the UI
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && !path.Contains("/swagger") && !path.Contains("/openapi"))
            {
                context.Response.Headers.Append(
                    "Content-Security-Policy",
                    "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;"
                );
            }

            // Referrer Policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions Policy (Limit browser features we don't need)
            context.Response.Headers.Append(
                "Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), payment=()"
            );

            // HSTS (Strict Transport Security) - Only apply in Production/HTTPS
            // if (context.Request.IsHttps)
            // {
            //     context.Response.Headers.Append(
            //         "Strict-Transport-Security", 
            //         "max-age=31536000; includeSubDomains"
            //     );
            // }

            await _next(context);
        }
    }
}
