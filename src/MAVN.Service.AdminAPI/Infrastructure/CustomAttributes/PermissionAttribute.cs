using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using MAVN.Service.AdminAPI.Domain.Enums;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MAVN.Service.AdminAPI.Infrastructure.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [PublicAPI]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IReadOnlyList<PermissionType> _permissionTypes;
        private readonly PermissionLevel _permissionLevel;
        private IExtRequestContext _requestContext;

        public PermissionAttribute(
            PermissionType permissionType,
            PermissionLevel permissionLevel)
        {
            _permissionTypes = new List<PermissionType> { permissionType };
            _permissionLevel = permissionLevel;
        }

        public PermissionAttribute(
            PermissionType[] permissionTypes,
            PermissionLevel permissionLevel)
        {
            _permissionTypes = permissionTypes.ToList();
            _permissionLevel = permissionLevel;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _requestContext = context.HttpContext.RequestServices.GetService<IExtRequestContext>();

            try
            {
                var hasPermission = await _requestContext.AdminHasPermissionAsync(_permissionTypes, _permissionLevel);

                if (!hasPermission)
                {
                    context.Result = new JsonResult(new {Error = "Not authorized"})
                    {
                        StatusCode = (int) HttpStatusCode.Forbidden
                    };
                }
            }
            catch(Exception ex)
            {
                var logFactory = context.HttpContext.RequestServices.GetService<ILogFactory>();
                var log = logFactory.CreateLog(this);
                log.Error(ex, context: new
                {
                    sessionToken = _requestContext.SessionToken,
                    url = context.HttpContext.Request.GetUri()
                });
            }
        }
    }
}
