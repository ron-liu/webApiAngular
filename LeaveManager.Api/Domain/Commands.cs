using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Domain
{
	public class ApplyLeaveCommand : ICommand, IValidatableObject
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

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if ((EndDate - StartDate).TotalDays > 100) yield return new ValidationResult("Cannot greater than 100 days");
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