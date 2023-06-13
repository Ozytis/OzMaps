using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace OzMaps.Web
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .SelectMany(s => s.Value.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage))
                    .ToArray();

                actionContext.Result = new ContentResult
                {
                    Content = JsonSerializer.Serialize(errors),
                    ContentType = "application/json",
                    StatusCode = 400
                };

                return;
            }

            base.OnActionExecuting(actionContext);
        }

    }
}
