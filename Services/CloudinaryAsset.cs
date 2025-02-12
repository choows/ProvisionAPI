using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using Newtonsoft.Json;
using ProvisionAPI.Models;
using ProvisionAPI.Models.AuthController;
using System.Net;
namespace ProvisionAPI.Services
{
	public class CloudinaryAsset : IAssetProcessing
	{
		private readonly Cloudinary cloudinary;
		private IProjectDbConn _projectDbConn;

		public CloudinaryAsset(IProjectDbConn projectDbConn)
		{
			DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
			cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
			cloudinary.Api.Secure = true;
			this._projectDbConn = projectDbConn;
		}

		public async Task<Tuple<string, long>> UploadImage(Stream imageStream, string Name, int uid)
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
			var result = await this.InsertIntoDb(uploadResult.Url.AbsoluteUri, JsonConvert.SerializeObject(uploadResult.JsonObj), (int)CustomAssetType.Image, uid);
			return (Tuple.Create<string,long>(uploadResult.Url.AbsoluteUri,result) );
		}

		private async Task<long> InsertIntoDb(string Url, string AdditionalInfo, int Type, int uid)
		{

			var id = long.Parse(uid.ToString() + DateTime.Now.Ticks.ToString());
			var SPName = "\"ProvisionProj\".insertAsset";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("id", id);
			parameters.Add("info", AdditionalInfo);
			parameters.Add("p", Url);
			parameters.Add("t", Type);
			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return id;
		}


	}
}
