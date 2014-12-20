using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace LeaveManager.Api
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError().GetBaseException();
			var apiException = exception as ApiException;
			var ret = new ApiModel();
			if (apiException == null) ret.Messages.Add(new Notification { Content = exception.Message, MessageType = Notification.Type.Error });
			else ret.Messages.AddRange(apiException.ErrorStrings.Select(x => new Notification { Content = x, MessageType = Notification.Type.Error }));

			Response.StatusCode = 400;
			Response.ContentType = "application/json";
			Response.Write(JsonConvert.SerializeObject(ret));
			Server.ClearError();
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}