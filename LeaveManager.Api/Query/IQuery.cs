using System.Security.Cryptography;

namespace LeaveManager.Api.Query
{
	public interface IQuery
	{ }

	public interface IQueryHandler<in TIn, out TOut> where TIn : IQuery
	{
		TOut Query(TIn input);
	}
}