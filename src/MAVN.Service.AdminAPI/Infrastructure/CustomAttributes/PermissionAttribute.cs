using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Common.Middleware.Authentication;
using JetBrains.Annotations;
using Lykke.Common.Log;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MAVN.Service.AdminAPI.Infrastructure.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [PublicAPI]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IReadOnlyList<PermissionType> _permissionTypes;
        private readonly PermissionLevel _permissionLevel;
        private IRequestContext _requestContext;
        private IAdminsService _adminsService;
        private ILog _log;

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
            var logFactory = (ILogFactory)context.HttpContext.RequestServices.GetService(typeof(ILogFactory));
            _log = logFactory.CreateLog(this);
            _requestContext = (IRequestContext)context.HttpContext.RequestServices.GetService(typeof(IRequestContext));
            _adminsService = (IAdminsService)context.HttpContext.RequestServices.GetService(typeof(IAdminsService));

            try
            {
                var hasPermission = await _adminsService.AdminHasPermissionAsync(_requestContext.UserId, _permissionTypes, _permissionLevel);

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
                _log.Error(ex, context: new
                {
                    sessionToken = _requestContext.SessionToken,
                    url = context.HttpContext.Request.GetUri()
                });
            }
        }
    }
}
