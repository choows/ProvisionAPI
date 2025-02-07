using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class LoginUser
	{
		[NotNull]
		[EmailAddress]
		public string Email { get; set; }
		[NotNull]
		public string Password { get; set; }
	}
}
