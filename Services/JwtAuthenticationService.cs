using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ProvisionAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProvisionAPI.Services
{
	public class JwtAuthenticationService  : IJwtAuthenticationService
	{
		private IConfiguration _appSettings;
		private string _jwtSettinsSecret;
		private TimeSpan _jwtTokenLifeTime;
		private readonly TokenValidationParameters _tokenValidationParameters;
		private readonly IProjectDbConn _projectDbConn;
		private readonly IAuthServices _authServices;

		public JwtAuthenticationService(IConfiguration appSettings, TokenValidationParameters tokenValidationParameters, IProjectDbConn projectDbConn, IAuthServices authServices)
		{
			_appSettings = appSettings;
			_jwtSettinsSecret = _appSettings.GetValue<string>("JwtSettings:Secret");
			_jwtTokenLifeTime = _appSettings.GetValue<TimeSpan>("JwtSettings:TokenLifetime");
			_tokenValidationParameters = tokenValidationParameters;
			_projectDbConn = projectDbConn;
			_authServices = authServices;
		}
		private async Task UpdateRefreshToken(int id)
		{
			var SPName = "\"ProvisionProj\".updateRefreshToken";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("id", id);
			var result = await _projectDbConn.ExecuteStoredProc(SPName, parameters);

		}
		private async Task<RefreshToken> getRefreshToken(string token)
		{
			//SELECT * FROM "ProvisionProj".getrefreshtoken('james');
			var SPName = "\"ProvisionProj\".getrefreshtoken";
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("t", token);
			var queryResult = await _projectDbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var result = DataTableToObject.ConvertDataTable<RefreshToken>(queryResult);
				return result.FirstOrDefault();
			}
			return null;
		}
		private async Task InsertRefreshToken(RefreshToken refreshToken)
		{
			var SPName = "\"ProvisionProj\".insert_new_jwt";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("t", refreshToken.Token);
			parameters.Add("jwtid", refreshToken.JwtId);
			parameters.Add("uid", refreshToken.UserId);
			parameters.Add("cdt", refreshToken.CreationDate);
			parameters.Add("expdt", refreshToken.ExpiryDate);
			parameters.Add("u", refreshToken.Used);
			var result = await _projectDbConn.ExecuteStoredProc(SPName, parameters);
		}

		private async Task<RefreshToken> GenerateRefrehToken(SecurityToken securitytoken, int UserId)
		{
			//insert into db for later refresh token
			var token = new RefreshToken
			{
				Token = Guid.NewGuid().ToString(),
				JwtId = securitytoken.Id,
				UserId = UserId,
				CreationDate = DateTime.Now,
				ExpiryDate = DateTime.Now.AddMonths(6)
			};
			await InsertRefreshToken(token);
			return token;
		}
		private ClaimsPrincipal GetPrincipalFromToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var tokenValidationParameters = _tokenValidationParameters.Clone();
				tokenValidationParameters.ValidateLifetime = false;
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
				if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
				{
					return null;
				}

				return principal;
			}
			catch
			{
				return null;
			}
		}

		private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
		{
			return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
				   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
					   StringComparison.InvariantCultureIgnoreCase);
		}

		public async Task<JwtToken> RegenerateRefreshToken(string token, string refreshToken)
		{
			var validatedToken = GetPrincipalFromToken(token);

			if (validatedToken == null)
			{
				throw new Exception("Invalid Token");
			}

			var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

			var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				.AddSeconds(expiryDateUnix);

			var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

			var storedRefreshToken = await getRefreshToken(refreshToken); 

			if (storedRefreshToken == null)
			{
				throw new Exception("This refresh token does not exist");
			}

			if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
			{
				throw new Exception("This refresh token has expired");
			}

			if (storedRefreshToken.Used == true)
			{
				throw new Exception("This refresh token has been used");
			}

			if (storedRefreshToken.JwtId != jti)
			{
				throw new Exception("This refresh token does not match this JWT");
			}

			storedRefreshToken.Used = true;
			await this.UpdateRefreshToken(storedRefreshToken.ID);

			string strUserId = validatedToken.Claims.Single(x => x.Type == "UserId").Value;
			long userId = 0;
			long.TryParse(strUserId, out userId);
			var user = await _authServices.GetUserInfoById((int)userId);

			if (user == null)
			{
				throw new Exception("User Not Found");
			}

			return await GenerateToken(user);
		}

		public async Task<JwtToken> GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var key = Encoding.ASCII.GetBytes(_jwtSettinsSecret);

				ClaimsIdentity Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim("UserId", user.ID.ToString()),
					new Claim("Email",user.Email==null?"":user.Email),
					new Claim("UserName",user.UserName==null?"":user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				});

				//no user role needed for now
				//may refer to https://www.c-sharpcorner.com/article/implement-jwt-in-net-core-api/
				//foreach (var item in GetUserRole(user.UserId))
				//{
				//	Subject.AddClaim(new Claim(ClaimTypes.Role, item.RoleName));
				//}

				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = Subject,
					Expires = DateTime.UtcNow.Add(_jwtTokenLifeTime),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var refreshToken = await GenerateRefrehToken(token, user.ID);
				return new JwtToken
				{
					Token = tokenHandler.WriteToken(token),
					refreshToken = refreshToken
				};
			}
			catch (Exception ex)
			{
				throw;
			}

		}
	}
}
