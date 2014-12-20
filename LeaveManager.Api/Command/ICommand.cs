namespace LeaveManager.Api.Command
{
	public interface ICommand
	{ }

	public interface ICommandHandler<in TIn> where TIn : ICommand
	{
		void Execute(TIn input);
	}
}