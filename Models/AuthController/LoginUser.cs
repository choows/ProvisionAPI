using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class LoginUser
	{
		[NotNull]
		[EmailAddress]
		[JsonProperty("email")]
		public string Email { get; set; }
		[NotNull]
		[JsonProperty("password")]
		public string Password { get; set; }
	}
}
