using System;
using System.Collections.Generic;

namespace LeaveManager.Api.Domain
{
	public interface IReadModel { }

	public class LeaveReadModel : IReadModel
	{
		public string Applicant { get; set; }
		public string Evaluator { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Leave.ReasonEnum Reason { get; set; }
		public Leave.StatusEnum Status { get; set; }
		public string ApplicantComment { get; set; }
		public string EvaluatorComment { get; set; }
		public DateTime AppliedOn { get; set; }
		public DateTime EvaluatedOn { get; set; }
	}
}