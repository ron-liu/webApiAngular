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
		[Inject] public IEventSteamRepository<Leave> repo { get; set; }

		public void Handle(ApplyLeaveCommand command)
		{
			var leave = new Leave(command.LeaveId);
			leave.ApplyLeave(command.UserName, command.StartDate, command.EndDate, command.Reason, command.Comment);
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