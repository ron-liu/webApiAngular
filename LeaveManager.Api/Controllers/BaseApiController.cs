using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace LeaveManager.Api.Controllers
{
	public class BaseApiController : ApiController
	{
		protected void CheckModelSate()
		{
			if (ModelState.IsValid) return;
			throw new ApiException(ModelState.SelectMany(x => x.Value.Errors.Select(y=>y.ErrorMessage)));
		}
	}
}