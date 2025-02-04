namespace ProvisionAPI.Services
{
	public interface ICustomEncryption
	{
		public string GenerateHashValue(string value);
	}
}
