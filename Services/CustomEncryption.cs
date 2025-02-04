using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace ProvisionAPI.Services
{
	public class CustomEncryption : ICustomEncryption
	{
		//to do hashing
		private byte[] SaltByte;
		public CustomEncryption(IConfiguration configuration)
		{
			//the salt must be exactly same as in appsettings 
			var salt = configuration.GetValue<string>("Salt");
			this.SaltByte = Encoding.ASCII.GetBytes(salt);
		}

		public string GenerateHashValue(string value)
		{
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: value,
			salt: SaltByte,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
			numBytesRequested: 256 / 8));
			return hashed;
		}
	}
}
