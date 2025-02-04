using ProvisionAPI.Models;
using ProvisionAPI.Models.AuthController;

namespace ProvisionAPI.Services
{
	public interface IAuthServices
	{
		public Task<bool> Register(RegisterUser registerUser);
		public Task<User> Login(string username, string password);
		public Task<User> GetUserInfoById(int Id);

	}
}
