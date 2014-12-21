using System;
using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Domain
{
	public class LeaveApplied : IEvent
	{
		public int WorkingDays { get; set; }
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Leave.ReasonEnum Reason { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }
		public Guid SourceId { get; set; }
		public int Version { get; set; }
	}

	public class LeaveEvaluated : IEvent
	{
		public string UserName { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }
		public bool IsApproved { get; set; }
		public Guid SourceId { get; set; }
		public int Version { get; set; }
	}

}