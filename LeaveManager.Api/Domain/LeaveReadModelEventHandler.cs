using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Domain
{
	public class LeaveReadModelEventHandler : IEventHandler<LeaveApplied>, IEventHandler<LeaveEvaluated>
	{
		public void Handle(LeaveApplied @event)
		{
			throw new System.NotImplementedException();
		}

		public void Handle(LeaveEvaluated @event)
		{
			throw new System.NotImplementedException();
		}
	}
}