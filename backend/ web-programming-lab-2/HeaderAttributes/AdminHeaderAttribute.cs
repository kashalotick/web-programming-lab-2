using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace web_programming_lab_2.HeaderAttributes;

public class AdminHeaderAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Administrator", out var headerValue) 
            || headerValue != "true")
        {
            context.Result = new ContentResult()
            {
                StatusCode = 403,
                Content = "Доступ заборонено: відсутній або невірний заголовок Administrator"
            };
        }
    }
}