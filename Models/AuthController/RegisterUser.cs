using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class RegisterUser
	{
		[NotNull]
		public string UserName { get; set; }
		[NotNull]
		public string Password { get; set; }
		[NotNull]
		public string ConfirmPassword { get; set; }
		[EmailAddress]
		public string Email { get; set; }

	}
}
