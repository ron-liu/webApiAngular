using System.Linq;
using NEventStore;
using NEventStore.Dispatcher;
using Ninject;

namespace LeaveManager.Api.Infrastructure
{
	// Used for NEventStore to publish events
	public class FakeBusDispatcher : IDispatchCommits
	{
		private IEventPublisher EventPublisher { get; set; }

		public FakeBusDispatcher(IEventPublisher eventPublisher)
		{
			EventPublisher = eventPublisher;
		}

		public void Dispose()
		{ }

		public void Dispatch(ICommit commit)
		{
			foreach (var e in commit.Events.Select(x=>x.Body))
			{
				EventPublisher.Publish(e as IEvent);
			}
		}
	}
}