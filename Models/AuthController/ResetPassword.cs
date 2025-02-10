using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class ChangePassword
	{
		[NotNull]
		[EmailAddress]
		public string Email { get; set; }
		[NotNull]
		public string Password { get; set; }
		[NotNull]
		public string NewPassword { get; set; }
		[NotNull]
		public string ConfirmPassword { get; set; }
	}
}
