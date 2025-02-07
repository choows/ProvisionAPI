using Microsoft.AspNetCore.Mvc;
using ProvisionAPI.Models.AuthController;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private IAuthServices _authServices;

		public AuthController(IAuthServices authServices)
		{
			_authServices = authServices;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterUser registerUser)
		{
			if (await _authServices.Register(registerUser))
			{
				return Ok();
			}
			else
			{
				return BadRequest("Unable to register user");
			}
		}

		[HttpPost("LoginViaEmail")]
		public async Task<IActionResult> LoginViaEmail(LoginUser loginCredential)
		{
			var usr = await _authServices.Login(loginCredential.Email, loginCredential.Password);

			if (usr != null)
			{
				return Ok(usr);
			}
			else
			{
				return BadRequest("Incorrect Email or Password");
			}
		}

		//[HttpPost("ChangePassword")]
		//public async Task<IActionResult> ChangePassword()
		//{

		//}
	}
}
