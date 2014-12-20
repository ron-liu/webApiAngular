using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LeaveManager.Api
{
	public class AuthContext : IdentityDbContext<IdentityUser>
	{
		public AuthContext()
			: base("AuthContext")
		{

		}
	}
}