using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using Newtonsoft.Json;
using System.Net;
namespace ProvisionAPI.Services
{
	public class CloudinaryAsset : IAssetProcessing
	{
		private readonly Cloudinary cloudinary;
		public CloudinaryAsset()
		{
			DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
			cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
			cloudinary.Api.Secure = true;
		}

		public async Task<string> UploadImage(Stream imageStream, string Name)
		{
			var ImageParams = new ImageUploadParams()
			{
				File = new FileDescription(Name, imageStream),
				UseFilename = false,
				UniqueFilename = true,
				Overwrite = true
			};

			var uploadResult = await cloudinary.UploadAsync(ImageParams);
			//uploadResult.JsonObj
			//able to record more information at this stage 
			//will create another DB for this, asset management 
			if(uploadResult.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception(uploadResult.Error.Message);
			}
			return (uploadResult.Url.AbsoluteUri);
		}

	}
}
