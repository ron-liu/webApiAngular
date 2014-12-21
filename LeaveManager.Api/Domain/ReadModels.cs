using System;
using System.Collections.Generic;

namespace LeaveManager.Api.Domain
{
	public class LeaveReadModel
	{
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Leave.ReasonEnum Reason { get; set; }
		public Leave.StatusEnum Status { get; set; }
		public List<Leave.CommentClass> Comments { get; set; }
		public DateTime AppliedOn { get; set; }
		public DateTime EvaluatedOn { get; set; }
	}
}