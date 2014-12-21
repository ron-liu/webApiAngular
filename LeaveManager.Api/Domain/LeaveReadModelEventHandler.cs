using LeaveManager.Api.Infrastructure;
using Ninject;

namespace LeaveManager.Api.Domain
{
	public class LeaveReadModelEventHandler : IEventHandler<LeaveApplied>, IEventHandler<LeaveEvaluated>
	{
		[Inject]
		public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

		public void Handle(LeaveApplied @event)
		{
			LeaveReadModelRepository.SyncLeave(@event.SourceId);
		}

		public void Handle(LeaveEvaluated @event)
		{
			LeaveReadModelRepository.SyncLeave(@event.SourceId);
		}
	}
}