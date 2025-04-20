using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhysicalPersonsDirectory.Api.Middleware;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
        var culture = "en-US"; 

        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            var languageCode = acceptLanguage.Split(',').FirstOrDefault()?.Split(';').FirstOrDefault();
            if (!string.IsNullOrEmpty(languageCode) && IsValidCulture(languageCode))
            {
                culture = languageCode;
            }
        }

        CultureInfo.CurrentCulture = new CultureInfo(culture);
        CultureInfo.CurrentUICulture = new CultureInfo(culture);

        await _next(context);
    }

    private bool IsValidCulture(string cultureName)
    {
        try
        {
            CultureInfo.GetCultureInfo(cultureName);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}