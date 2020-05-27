using Arise.DDD.API;
using AuthService.Application.Commands.RegisterUserName;
using AuthService.Data;
using AuthService.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator, UserManager<ApplicationUser> userManager, ILogger<RegisterController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user == null ? true : false));
        }

        [HttpPost]
        [Route("username")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> RegisterWithUserName([FromBody] RegisterUserNameCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                var messageBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                    messageBuilder = messageBuilder.Append(error.Description).Append("\r\n");

                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, messageBuilder.ToString()));
            }

            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }
    }
}
