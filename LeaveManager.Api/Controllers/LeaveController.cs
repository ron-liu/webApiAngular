using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Http;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;
using LeaveManager.Api.Models;
using Ninject;

namespace LeaveManager.Api.Controllers
{
	public class LeaveController : BaseApiController
	{
		[Inject] private IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>> ListLeavesByUserName { get; set; }
		[Inject] private IQueryHandler<ListLeavesToEvaluate, IEnumerable<Leave>> ListLeavesToEvaluate { get; set; }
		[Inject] private IQueryHandler<LeaveById, Leave> LeaveById { get; set; }
		[Inject] private IQueryHandler<MyLeaveById, Leave> MyLeaveById { get; set; }
		[Inject] private IQueryHandler<GetWorkingDays, int> WorkingDays { get; set; }
		[Inject] private IQueryHandler<OverlapWithApprovedLeaves, OverlapWithApprovedLeaves.OverlapCheckResult> OverlapWithApprovedLeaves { get; set; }
		[Inject] private ICommandSender CommandSender { get; set; }

		[Route("apply")]
		[Authorize]
		[HttpPost]
		public IHttpActionResult Apply(ApplyLeaveCommand command)
		{
			CheckModelSate();

			var workingDays = WorkingDays.Query(new GetWorkingDays {EndDate = command.EndDate, StartDate = command.StartDate});
			if (workingDays <= 0) throw new ApiException("Cannot apply for 0 working days");

			var overlappedResult = OverlapWithApprovedLeaves.Query(new OverlapWithApprovedLeaves{StartDate = command.StartDate, EndDate = command.EndDate});
			if (overlappedResult.Overlaped)
				throw new ApiException(string.Format("Overlapped with existing Leave: start at: {0}, end at: {1}", overlappedResult.OverlappedLeave.StartDate, overlappedResult.OverlappedLeave.EndDate));

			command.UserName = User.Identity.Name;
			CommandSender.Send(command);
			return Content(HttpStatusCode.OK, 0);
		}

		[Route("list-my-leaves")]
		[Authorize]
		[HttpGet]
		public IHttpActionResult List()
		{
			return Content(HttpStatusCode.OK, ListLeavesByUserName.Query(new ListLeavesByUserName { UserName = User.Identity.Name }));
		}

		[Route("leave/{leaveId}")]
		[Authorize(Roles = "manager")]
		[HttpGet]
		public IHttpActionResult GetLeaveById(Guid leaveId)
		{
			return Content(HttpStatusCode.OK, LeaveById.Query(new LeaveById {LeaveId = leaveId}));
		}

		[Route("my-leave/{leaveId}")]
		[Authorize]
		[HttpGet]
		public IHttpActionResult GetMyLeaveById(Guid leaveId)
		{
			return Content(HttpStatusCode.OK, MyLeaveById.Query(new MyLeaveById { LeaveId = leaveId, UserName = User.Identity.Name}));
		}

		[Route("Evaluate")]
		[Authorize(Roles = "manager")]
		[HttpPost]
		public IHttpActionResult Evaluate(EvaluateLeaveCommand command)
		{
			command.UserName = User.Identity.Name;
			CommandSender.Send(command);
			return Content(HttpStatusCode.OK, 0);
		}

		[Route("list-to-evaluate")]
		[Authorize(Roles = "manager")]
		[HttpGet]
		public IHttpActionResult ListToEvaluate()
		{
			return Content(HttpStatusCode.OK, ListLeavesToEvaluate.Query(new ListLeavesToEvaluate()));
		}

		[HttpGet]
		[Route("options")]
		public IHttpActionResult Options()
		{
			return Content(HttpStatusCode.OK, new
			{
				ReasonOptions = Enum.GetValues(typeof(Leave.ReasonEnum)).Cast<Leave.ReasonEnum>().Select(v => new { key = (int)v, name = v.GetDescription() }),
			});
		}

		[HttpPost]
		[Route("working-days")]
		public IHttpActionResult GetWorkingDays(WorkingDaysModel model)
		{
			return Content(HttpStatusCode.OK,
				WorkingDays.Query(new GetWorkingDays {StartDate = model.StartDate, EndDate = model.EndDate}));
		}
	}
}