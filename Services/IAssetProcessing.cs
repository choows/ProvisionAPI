namespace ProvisionAPI.Services
{
	public interface IAssetProcessing
	{
		public Task<Tuple<string, long>> UploadImage(Stream imageStream, string Name, int uid);
	}
}