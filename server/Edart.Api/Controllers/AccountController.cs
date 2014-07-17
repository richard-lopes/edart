using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Edart.Api.Controllers;
using Edart.Common;

namespace edart.api.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/

        [HttpPost]
        public HttpResponseMessage Login(string uid, string pwd)
        {
            var loginModel = new Login();
            loginModel.Token = uid;
            return Request.CreateResponse(HttpStatusCode.OK, loginModel);
        }

    }
}
