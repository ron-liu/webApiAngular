using System.Threading.Tasks;
using System.Web.Http;
using LeaveManager.Api.Models;
using Microsoft.AspNet.Identity;

namespace LeaveManager.Api.Controllers
{
	public class AccountController : BaseApiController
	{
		private AuthRepository _repo = null;

		public AccountController()
		{
			_repo = new AuthRepository();
		}

		// POST api/Account/Register
		[AllowAnonymous]
		[Route("sign-up")]
		[HttpPost]
		public async Task<IHttpActionResult> Register(UserModel userModel)
		{
			CheckModelSate();

			var result = await _repo.RegisterUser(userModel);

			var errorResult = GetErrorResult(result);

			return errorResult ?? Ok();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) _repo.Dispose();
			base.Dispose(disposing);
		}

		private IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null) return InternalServerError();

			if (result.Succeeded) return null;
			if (result.Errors != null)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error);
			}
			CheckModelSate();
			return BadRequest(); // No ModelState errors are available to send, so just return an empty BadRequest.
		}

		
		//[Authorize]
		[HttpGet]
		public IHttpActionResult List()
		{
			//return Content(HttpStatusCode.OK, 
			//	listLeavesByUserName.Query(new ListLeavesByUserNameCondition{UserName = User.Identity.Name}));
			return null;
		}
	}
}