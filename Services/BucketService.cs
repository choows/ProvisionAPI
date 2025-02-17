using CloudinaryDotNet;
using ProvisionAPI.Models;
using ProvisionAPI.Models.BucketController;
using System.Net.Sockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
			parameters.Add("rid", bucket.RecipeID.HasValue ? bucket.RecipeID.Value : -1);
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
		}
		public async Task<bool> UpdateBucket(List<UpdateBucket> updateBuckets, int uid)
		{
			//just update and ignore the previous one 
			//if wanna to remove, just set the required to 0
			var result = false;

			foreach (var bucket in updateBuckets)
			{
				var purchaseDate = DateOnly.Parse(bucket.PurchaseDate.ToString("yyyy-MM-dd"));
				foreach (var ingredient in bucket.ingredients)
				{
					result = await this.UpdateSingleIngredientFromBucket(uid, ingredient.Purchased, ingredient.Required, ingredient.IngredientId, null, purchaseDate);
				}
			}
			return result;
		}

		private async Task<bool> UpdateSingleIngredientFromBucket(int uid, double purchased, double required, int ingredientId, int? RecipeId, DateOnly purchaseDate)
		{
			var SPName = "\"ProvisionProj\".updatebucket";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			//"ProvisionProj".insertIntoBucket(uid integer, iid integer, Rid integer, ureq DOUBLE PRECISION, up DOUBLE PRECISION, d DATE)
			parameters.Add("uid", uid);
			parameters.Add("iid", ingredientId);
			parameters.Add("rid", RecipeId.HasValue ? RecipeId.Value : -1);
			parameters.Add("ureq", required);
			parameters.Add("up", purchased);
			parameters.Add("d", purchaseDate);

			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}

	}
}
