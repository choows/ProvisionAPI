using ProvisionAPI.Models;

namespace ProvisionAPI.Services
{
	public interface IJwtAuthenticationService
	{
		public Task<JwtToken> GenerateToken(User user);
	}
}