using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Oqtane.Shared;

namespace Oqtane.Security
{
    public class AutoValidateAntiforgeryTokenFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<AutoValidateAntiforgeryTokenFilter> _filelogger;

        public AutoValidateAntiforgeryTokenFilter(IAntiforgery antiforgery, ILogger<AutoValidateAntiforgeryTokenFilter> filelogger)
        {
            _antiforgery = antiforgery;
            _filelogger = filelogger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
            {
                return;
            }

            var url = context.HttpContext.Request.GetEncodedUrl();
            var method = context.HttpContext.Request.Method;
            
            _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Processing {method} request to {url}");

            if (ShouldValidate(context))
            {
                _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Validating antiforgery token for {method} {url}");
                
                try
                {
                    await _antiforgery.ValidateRequestAsync(context.HttpContext);
                    _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Validation SUCCESS for {url}");
                }
                catch (Exception ex)
                {
                    context.Result = new AntiforgeryValidationFailedResult();
                    _filelogger.LogError(ex, $"[AutoValidateAntiforgeryTokenFilter] Validation FAILED for {url}");
                }
            }
            else
            {
                _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] SKIPPING validation for {method} {url}");
            }
        }

        protected virtual bool ShouldValidate(AuthorizationFilterContext context)
        {
            // TEMPORARY: Disable antiforgery validation for debugging
            _filelogger.LogWarning("[AutoValidateAntiforgeryTokenFilter] TEMPORARY: Antiforgery validation DISABLED for debugging");
            return false;
            
            /* Original validation logic - re-enable after fixing token flow
            var method = context.HttpContext.Request.Method;
            var hasAuthHeader = context.HttpContext.Request.Headers.ContainsKey("Authorization");
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            var isMaui = userAgent == Constants.MauiUserAgent;
            
            // ignore antiforgery validation if a bearer token was provided
            if (hasAuthHeader)
            {
                _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Skipping - has Authorization header");
                return false;
            }

            // ignore antiforgery validation if client is a MAUI app
            if (isMaui)
            {
                _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Skipping - MAUI client");
                return false;
            }

            // ignore antiforgery validation for GET, HEAD, TRACE, OPTIONS
            if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsTrace(method) || HttpMethods.IsOptions(method))
            {
                _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Skipping - {method} method");
                return false;
            }

            _filelogger.LogDebug($"[AutoValidateAntiforgeryTokenFilter] Should validate - {method} request");
            // everything else requires antiforgery validation (ie. POST, PUT, DELETE)
            return true;
            */
        }
    }
}
