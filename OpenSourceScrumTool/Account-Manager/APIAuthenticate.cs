using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Account_Manager
{
    public class ApiAuthenticate : AuthorizeAttribute
    {
        private RoleEnum.Roles _validRole;
        private UserAuthenticate _authenticate = new UserAuthenticate();

        private string _responseReason;
        public ApiAuthenticate()
        {
        }
        public ApiAuthenticate(RoleEnum.Roles requestedRole)
        {
            _validRole = requestedRole;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (_authenticate.CheckAuth(actionContext.RequestContext.Principal, _validRole))
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                LogHelper.ErrorLog("Invalid API Access, Attempted to access " + actionContext.Request.RequestUri.ToString());
                _responseReason =
                    "Access Denied, This attempt has been logged. if you believe this to be in error, Please contact your administrator";
                this.HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            actionContext.Response.ReasonPhrase = !string.IsNullOrEmpty(_responseReason) ? _responseReason : "";
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return _authenticate.CheckAuth(actionContext.RequestContext.Principal, _validRole);
        }
    }
}