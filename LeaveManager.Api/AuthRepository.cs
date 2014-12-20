using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LeaveManager.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LeaveManager.Api
{
	public class AuthRepository : IDisposable
	{
		private AuthContext _ctx;

		private UserManager<IdentityUser> _userManager;

		public AuthRepository()
		{
			_ctx = new AuthContext();
			_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
		}

		public async Task<IdentityResult> RegisterUser(UserModel userModel)
		{
			var user = new IdentityUser
			{
				UserName = userModel.UserName
			};

			var result = await _userManager.CreateAsync(user, userModel.Password);
			return result;
		}

		public async Task<IdentityUser> FindUser(string userName, string password)
		{
			var user = await _userManager.FindByNameAsync(userName);
			if (user == null) return null;

			var result = _userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
			return result == PasswordVerificationResult.Failed ? null : user;
		}

		public void Dispose()
		{
			_ctx.Dispose();
			_userManager.Dispose();
		}
	}
}