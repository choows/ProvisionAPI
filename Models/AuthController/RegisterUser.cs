using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class RegisterUser
	{
		[NotNull]
		[JsonProperty("username")]
		public string UserName { get; set; }
		[NotNull]
		[JsonProperty("password")]
		public string Password { get; set; }
		[NotNull]
		[JsonProperty("confirmPassword")]
		public string ConfirmPassword { get; set; }
		[EmailAddress]
		[JsonProperty("email")]
		public string Email { get; set; }

	}
}
