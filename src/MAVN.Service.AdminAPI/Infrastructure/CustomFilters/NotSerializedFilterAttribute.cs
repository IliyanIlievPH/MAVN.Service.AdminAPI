using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MAVN.Service.AdminAPI.Infrastructure.CustomFilters
{
    public class NotSerializedFilterAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        public NotSerializedFilterAttribute() :
            this(arguments => arguments.ContainsValue(null))
        {

        }

        public NotSerializedFilterAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_validate(context.ActionArguments as Dictionary<string, object>))
            {
                context.Result = new BadRequestObjectResult("Object can not be serialized!");
            }
        }
    }
}
