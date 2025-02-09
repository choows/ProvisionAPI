using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.AuthController
{
	public class ReAuthenticateUser
	{
		[NotNull]
		[JsonProperty("token")]
		public string Token { get; set; }
		[NotNull]
		[JsonProperty("refreshToken")]
		public string RefreshToken { get; set; }
	}
}
