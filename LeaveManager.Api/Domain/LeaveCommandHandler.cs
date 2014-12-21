using System;
using System.IO;
using System.Linq;
using LeaveManager.Api.Infrastructure;
using NEventStore;
using Ninject;

namespace LeaveManager.Api.Domain
{
	public class LeaveCommandHandler : ICommandHandler<ApplyLeaveCommand>, ICommandHandler<EvaluateLeaveCommand>
	{
		[Inject] private IEventSteamRepository<Leave> repo { get; set; }
		[Inject] private IQueryHandler<GetWorkingDays, int> WorkingDays { get; set; }

		public void Handle(ApplyLeaveCommand command)
		{
			var leave = new Leave(command.LeaveId);
			var workingDays = WorkingDays.Query(new GetWorkingDays { StartDate = command.StartDate, EndDate = command.EndDate});
			leave.ApplyLeave(command.UserName, command.StartDate, command.EndDate, workingDays, command.Reason, command.Comment);
			repo.Save(leave);
		}

		public void Handle(EvaluateLeaveCommand command)
		{
			var leave = repo.Get(command.LeaveId);
			leave.EvaluateLeave(command.UserName, command.IsApproved, command.Comment);
			repo.Save(leave);
		}
	}
}