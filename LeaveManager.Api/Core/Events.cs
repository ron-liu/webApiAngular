using System;

namespace LeaveManager.Api.Core
{
	public class Event : IMessage
	{
		public Guid SourceId { get; set; }
		public int Version { get; set; }
	}

	public class LeaveApplied : Event
	{
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Leave.ReasonEnum Reason { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }
	}

	public class LeaveEvaluated : Event
	{
		public string UserName { get; set; }
		public string Comment { get; set; }
		public DateTime HappenedOn { get; set; }
		public bool IsApproved { get; set; }
	}

}