using CloudinaryDotNet;
using ProvisionAPI.Models;
using ProvisionAPI.Models.BucketController;

namespace ProvisionAPI.Services
{
	public class BucketService : IBucketService
	{
		private IProjectDbConn _projectDbConn;

		public BucketService(IProjectDbConn projectDbConn)
		{
			this._projectDbConn = projectDbConn;
		}

		public async Task<bool> InsertIntoBucket(AddBucket bucket, int uid)
		{
			var SPName = "\"ProvisionProj\".insertintobucket";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			//"ProvisionProj".insertIntoBucket(uid integer, iid integer, Rid integer, ureq DOUBLE PRECISION, up DOUBLE PRECISION, d DATE)
			parameters.Add("uid", uid);
			parameters.Add("iid", bucket.IngredientID);
			parameters.Add("rid", bucket.RecipeID.HasValue ? bucket.RecipeID : -1);
			parameters.Add("ureq", bucket.UnitRequired);
			parameters.Add("up", bucket.UnitPurchased);
			parameters.Add("d", bucket.PurchaseDate);

			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}

		public async Task<List<GetBucketByRange>> GetBucketByDateRange(DateOnly from, DateOnly to, int uid)
		{
			var SPName = "\"ProvisionProj\".getbucketbydaterange";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("from", from);
			parameters.Add("to", to);
			parameters.Add("uid", uid);
			var queryResult = await this._projectDbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var result = DataTableToObject.ConvertDataTable<GetBucketByRange>(queryResult);
				return result;
			}
			return null;
		}s
	}
}
