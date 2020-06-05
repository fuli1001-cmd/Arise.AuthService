using Arise.DDD.API;
using Arise.DDD.API.Response;
using AuthService.Application.Commands.ChangePassword;
using AuthService.Application.Commands.RegisterPhone;
using AuthService.Application.Commands.RegisterUserName;
using AuthService.Application.Commands.ResetPassword;
using AuthService.Application.Commands.ValidateUserName;
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

        /// <summary>
        /// 检查用户名是否可用
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> CheckUserNameAsync(string userName)
        {
            var command = new ValidateUserNameCommand { UserName = userName };
            try
            {
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, ex.Message, false));
            }

            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }
        
        /// <summary>
        /// 用户名密码注册
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("username")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> RegisterWithUserName([FromBody] RegisterUserNameCommand command)
        {
            try
            {
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, ex.Message));
            }

            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }

        /// <summary>
        /// 电话号码注册
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("phone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> RegisterWithPhoneNumber([FromBody] RegisterPhoneCommand command)
        {
            try
            {
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, ex.Message));
            }

            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }

        ///// <summary>
        ///// 邀请码注册
        ///// </summary>
        ///// <param name="command"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("code")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<bool>> RegisterWithCode([FromBody] RegisterUserNameCommand command)
        //{
        //    var result = await _mediator.Send(command);

        //    if (!result.Succeeded)
        //    {
        //        var messageBuilder = new StringBuilder();
        //        foreach (var error in result.Errors)
        //            messageBuilder = messageBuilder.Append(error.Description).Append("\r\n");

        //        return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, messageBuilder.ToString()));
        //    }

        //    return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        //}

        /// <summary>
        /// 检查用户的密保问题和密保答案是否匹配
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userName}/{question}/{answer}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ValidateSecretQuestion(string userName, string question, string answer)
        {
            var command = new ValidateSecretQuestionCommand { UserName = userName, SecretQuestion = question, SecretAnswer = answer };
            var user = await _mediator.Send(command);

            if (user == null)
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, "密保验证失败。"));

            return Ok(ResponseWrapper.CreateOkResponseWrapper(user == null ? false : true));
        }

        /// <summary>
        /// 获取用户的密保问题
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("secret/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetSecretQuestion(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return Ok(ResponseWrapper.CreateOkResponseWrapper(user?.SecretQuestion));
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("password/reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, ex.Message));
            }
            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("password/change")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseWrapper.CreateErrorResponseWrapper((int)HttpStatusCode.BadRequest, ex.Message));
            }
            return Ok(ResponseWrapper.CreateOkResponseWrapper(true));
        }

    }
}
