using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IngredientController : ControllerBase
	{
		private IJwtAuthenticationService _jwtAuthenticationService;

		public IngredientController(IJwtAuthenticationService jwtAuthenticationService)
		{
			_jwtAuthenticationService = jwtAuthenticationService;
		}


	}
}
