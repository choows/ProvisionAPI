using ProvisionAPI.Models;

namespace ProvisionAPI.Services
{
	public interface IJwtAuthenticationService
	{
		public Task<JwtToken> GenerateToken(User user);
		public Task<JwtToken> RegenerateRefreshToken(string token, string refreshToken);
		public int GetUserIDFromToken(string token);
	}
}