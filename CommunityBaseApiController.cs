using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Community.Api.Extensions;
using Community.Core;
using Community.Core.Entities.CommunityUsers;
using Community.Core.Infrastructure;
using Community.Services.CommunityUsers;
using Community.Services.Logging;
using Community.Services.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Community.Api.Controllers
{
    /// <summary>
    /// Holds common functionality for all the derived controllers
    /// </summary>
    [ApiController]
    public abstract class CommunityBaseApiController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected CommunityBaseApiController()
        {
            //TODO
        }


        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override OkObjectResult Ok(object value)
        {
            var data = CreateResponse(value, StatusCodes.Status200OK);
            return base.Ok(data);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            return CommunityBadRequest(modelState: modelState);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected ObjectResult AccessDenied(string value = null)
        {
            var responseMessageService = CommunityEngineContext.Current.Resolve<ICommunityResponseMessageService>();
            responseMessageService.AddMessage("You are not allowed to access this resource!");
            return StatusCode(StatusCodes.Status403Forbidden, value);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// MIGHT BE HELPFUL WHEN RETURNING forbid result , Not found result
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override ObjectResult StatusCode(int statusCode, object value)
        {
            var data = CreateResponse(value, statusCode);
            return base.StatusCode(statusCode, data);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override CreatedResult Created(Uri uri, object value)
        {
            var data = CreateResponse(value, StatusCodes.Status201Created);
            return base.Created(uri, data);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override UnauthorizedObjectResult Unauthorized(object value)
        {
            var data = CreateResponse(value, StatusCodes.Status401Unauthorized);
            return base.Unauthorized(data);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override BadRequestObjectResult BadRequest(object value)
        {
            var data = CreateResponse(value, StatusCodes.Status400BadRequest);
            return base.BadRequest(data);
        }

        /// <summary>
        /// Wrap the actual object in community response structured object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override NotFoundObjectResult NotFound(object value)
        {
            var data = CreateResponse(value, StatusCodes.Status404NotFound);
            return base.NotFound(data);
        }

        /// <summary>
        /// "This should not be used. This will not respond with community response structure.";
        /// </summary>
        /// <returns></returns>
        public override OkResult Ok()
        {
            return base.Ok();
        }

        /// <summary>
        /// "This should not be used. This will not respond with community response structure.";
        /// </summary>
        /// <returns></returns>
        public override BadRequestResult BadRequest()
        {
            throw new CommunityException("Not supported");
        }

        /// <summary>
        /// "This should not be used. This will not respond with community response structure.";
        /// </summary>
        /// <returns></returns>
        public override ForbidResult Forbid()
        {
            throw new CommunityException("Not supported");
        }

        /// <summary>
        /// "This should not be used. This will not respond with community response structure.";
        /// </summary>
        /// <returns></returns>
        public override NoContentResult NoContent()
        {
            throw new CommunityException("Not supported");
        }

        /// <summary>
        /// "This should not be used. This will not respond with community response structure.";
        /// </summary>
        /// <returns></returns>
        public override NotFoundResult NotFound()
        {
            throw new CommunityException("Not supported");
        }

        public static BadRequestObjectResult CommunityBadRequest(ModelStateDictionary modelState)
        {
            var problems = CreateResponse(null, StatusCodes.Status400BadRequest, modelState);
            return new BadRequestObjectResult(problems) { ContentTypes = { CommunityConstants.MimeTypes.ApplicationProblemJson, CommunityConstants.MimeTypes.ApplicationProblemXml } };
        }

        private static CommunityApiResponse<object> CreateResponse(object result, int statusCode, ModelStateDictionary modelState = null)
        {
            var notificationService = CommunityEngineContext.Current.Resolve<ICommunityResponseMessageService>();
            var messages = notificationService.GetMessages();
            if (modelState != null)
                messages.AddRange(modelState.SerializeErrors());
            return new CommunityApiResponse<object>(result, statusCode, messages);
        }

        protected CommunityUser GetAuthenticatedUserByBasicAuthenticationScheme()
        {
            var auth = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth))
                return null;

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            var communityUserRegistrationService = CommunityEngineContext.Current.Resolve<ICommunityUserRegistrationService>();
            CommunityUser user;
            UserLoginResults loginResult = communityUserRegistrationService.ValidateCommunityUser(username, password, null, out user);

            if (loginResult == UserLoginResults.WrongPassword || loginResult == UserLoginResults.NotActive || loginResult == UserLoginResults.LockedOut)
                return null;

            if (loginResult == UserLoginResults.Successful)
            {
                var userRole = CommunityCommonHelper.GetPriorityRoleFromRoles(user.GetCommunityUserRolesNameArray(null));

                if (string.IsNullOrEmpty(userRole) && userRole.ToLower() != "admin") //TODO
                {
                    return null;
                }

            }
            return user;
        }
    }
}