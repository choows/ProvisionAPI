namespace ProvisionAPI.Services
{
	public interface IAssetProcessing
	{
		public Task<string> UploadImage(Stream imageStream, string Name);
	}
}