using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProvisionAPI.Models;
using ProvisionAPI.Models.AuthController;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private IAuthServices _authServices;
		private IJwtAuthenticationService _jwtAuthenticationService;
		public AuthController(IAuthServices authServices, IJwtAuthenticationService jwtAuthenticationService)
		{
			_authServices = authServices;
			_jwtAuthenticationService = jwtAuthenticationService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterUser registerUser)
		{
			if (await _authServices.Register(registerUser))
			{
				return Ok("User Register Successfully");
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
				var jwtToken = await _jwtAuthenticationService.GenerateToken(usr);

				return Ok(new
				{
					UserId = usr.ID,
					UserEmail = usr.Email,
					UserName = usr.UserName,
					Token = jwtToken.Token,
					PasswordExpiry = usr.PasswordExpiry,
					RefreshToken = jwtToken.refreshToken.Token
				});
			}
			else
			{
				return BadRequest("Incorrect Email or Password");
			}
		}

		[HttpPost("ReAuthenticate")]
		public async Task<IActionResult> ReAuthenticate(ReAuthenticateUser reAuthenticateUser)
		{
			try
			{
				var jwtToken = await _jwtAuthenticationService.RegenerateRefreshToken(reAuthenticateUser.Token, reAuthenticateUser.RefreshToken);
				return Ok(new
				{
					Token = jwtToken.Token,
					RefreshToken = jwtToken.refreshToken.Token
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("ChangePassword")]
		[Authorize]
		public async Task<IActionResult> Logout(ChangePassword changePassword)
		{
			try
			{
				if (await this._authServices.UpdatePassword(changePassword.Password, changePassword.ConfirmPassword, changePassword.NewPassword, changePassword.Email))
				{
					return Ok("Done change password");
				}
				throw new Exception("Unable to change password");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
