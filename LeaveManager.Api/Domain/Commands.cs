using System;
using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Domain
{
	public class ApplyLeaveCommand : ICommand
	{
		public Guid LeaveId { get; set; }
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Leave.ReasonEnum Reason { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }

		public ApplyLeaveCommand()
		{
			LeaveId = Guid.NewGuid();
		}
	}

	public class EvaluateLeaveCommand : ICommand
	{
		public Guid LeaveId { get; set; }
		public string UserName { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }
		public bool IsApproved { get; set; }
	}
}