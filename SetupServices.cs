using Microsoft.IdentityModel.Tokens;
using ProvisionAPI.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProvisionAPI
{
	public static class SetupServices
	{
		//add dependency 
		/**
		 * Transient => always different; a new instance is provided to every controller and every service
		 * scoped => objects are the same within a request
		 * singletion => same for every object and every request 
		 * refer to https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
		 */

		public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IAuthServices, AuthServices>();
			services.AddScoped<IMiscServices, MiscServices>();
			services.AddScoped<IProjectDbConn>(x =>
				new ProjectDbConn(configuration.GetConnectionString("ProjectDB")));
			services.AddScoped<ICustomEncryption, CustomEncryption>();
			services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
		}

		public static void RegisterJwtAuth(IServiceCollection services, IConfiguration configuration)
		{
			var JwtSecretkey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtSettings:Secret"));
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(JwtSecretkey),
				ValidateIssuer = false,
				ValidateAudience = false,
				RequireExpirationTime = false,
				ValidateLifetime = true
			};
			services.AddSingleton(tokenValidationParameters);
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = tokenValidationParameters;

			});
		}
	}
}
