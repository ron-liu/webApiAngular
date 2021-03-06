﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using System.Web.Services.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaveManager.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

			if (!AppConfig.Debug)
				config.Services.Replace(typeof(IExceptionHandler), new ApplicationExceptionHandler());
        }
    }

	public static class AppConfig
	{
		public static bool Debug { get; set; }
		public static string Manager { get; set; }

		static AppConfig()
		{
			Debug = ConfigurationManager.AppSettings["debug"].ToLower() == "true";
			Manager = ConfigurationManager.AppSettings["manager"];
		}
	}

	public class ApplicationExceptionHandler : ExceptionHandler
	{
		public override void Handle(ExceptionHandlerContext context)
		{
			var negotiator = context.RequestContext.Configuration.Services.GetService(typeof(IContentNegotiator)) as IContentNegotiator;
			var formatters = context.RequestContext.Configuration.Formatters;

			var exception = context.Exception as ApiException;
			var ret = new ApiModel();
			if (exception == null) ret.Messages.Add(new Notification { Content = context.Exception.Message, MessageType = Notification.Type.Error });
			else ret.Messages.AddRange(exception.ErrorStrings.Select(x => new Notification { Content = x, MessageType = Notification.Type.Error }));

			context.Result = new NegotiatedContentResult<ApiModel>(exception != null ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError, ret, negotiator, context.Request, formatters);
			base.Handle(context);
		}
	}

	public class ApiModel
	{
		public List<Notification> Messages { get; set; }

		public ApiModel()
		{
			Messages = new List<Notification>();
		}
	}

	public class ApiModel<TContent> : ApiModel
	{
		public TContent Content { get; set; }
	}

	public class Notification
	{
		public string Content { get; set; }
		public Type MessageType { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public enum Type
		{
			Info = 0, Warning, Error
		}
	}

	public class ApiException : Exception
	{
		public IEnumerable<string> ErrorStrings { get; set; }

		public ApiException()
		{
			ErrorStrings = new List<string>();
		}
		public ApiException(string msg)
		{
			ErrorStrings = new List<string> {msg};
		}

		public ApiException(IEnumerable<string> msgs )
		{
			ErrorStrings = msgs;
		}

	}

}
