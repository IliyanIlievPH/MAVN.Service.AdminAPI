using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.AdminAPI.Controllers
{
    [Route("")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Redirects to swagger ui.
        /// </summary>
        [HttpGet]
        [SwaggerOperation("SwaggerRedirect")]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.Redirect)]
        public IActionResult Home()
        {
            return LocalRedirect("/swagger/ui/index.html");
        }
    }
}
