using Newtonsoft.Json;
using ProvisionAPI.Models;
using ProvisionAPI.Models.AuthController;
using System.ComponentModel.DataAnnotations;

namespace ProvisionAPI.Services
{
	public class AuthServices : IAuthServices
	{
		private IProjectDbConn _dbConn;
		private ICustomEncryption _customEncryption;
		public AuthServices(IProjectDbConn projectDbConn, ICustomEncryption customEncryption)
		{
			this._dbConn = projectDbConn;
			this._customEncryption = customEncryption;
		}

		public Task<User> GetUserInfoById(int Id)
		{
			throw new NotImplementedException();
		}

		public async Task<User> Login(string username, string password)
		{
			
			var entryPassword = this._customEncryption.GenerateHashValue(password);
			var usr = await getUserByEmailandPassword(username, entryPassword);

			if (usr != null)
			{
				return usr;
			}

			return null;
		}
		private bool ValidateEmail(string Email)
		{
			return new EmailAddressAttribute().IsValid(Email);
		}
		public async Task<bool> Register(RegisterUser registerUser)
		{
			var SPName = "\"ProvisionProj\".insert_new_user";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			//validate
			if (string.IsNullOrEmpty(registerUser.UserName) ||
				string.IsNullOrEmpty(registerUser.Password) ||
				string.Compare(registerUser.Password, registerUser.ConfirmPassword, false) != 0 ||
				string.IsNullOrEmpty(registerUser.Email)
				|| !ValidateEmail(registerUser.Email)
				)
			{
				throw new Exception(string.Format("Possible Null value in registration : {0}", JsonConvert.SerializeObject(registerUser)));
			}

			//check user existed 
			var usr = await getUserByEmail(registerUser.Email);
			if (usr != null)
			{
				throw new Exception("User Email Registered");
			}

			var password = this._customEncryption.GenerateHashValue(registerUser.Password);
			parameters.Add("un", registerUser.UserName);
			parameters.Add("p", password);
			parameters.Add("e", registerUser.Email);
			var result = await this._dbConn.ExecuteStoredProc(SPName, parameters);

			return true;
		}
		private async Task<User> getUserByEmail(string email)
		{
			var SPName = "\"ProvisionProj\".getUserByEmail";
			if (string.IsNullOrEmpty(email))
			{
				return null;
			}
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("e", email);
			var queryResult = await this._dbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var usrs = DataTableToObject.ConvertDataTable<User>(queryResult);
				return usrs.FirstOrDefault();
			}
			return null;
		}
		private async Task<User> getUserByEmailandPassword(string email, string password)
		{
			var SPName = "\"ProvisionProj\".getUserByEmailandPassword";
			if (string.IsNullOrEmpty(email))
			{
				return null;
			}
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("e", email);
			parameters.Add("p", password);

			var queryResult = await this._dbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var usrs = DataTableToObject.ConvertDataTable<User>(queryResult);
				return usrs.FirstOrDefault();
			}
			return null;
		}
	}
}
