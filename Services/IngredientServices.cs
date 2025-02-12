using ProvisionAPI.Models;
using ProvisionAPI.Models.IngredientController;

namespace ProvisionAPI.Services
{
	public class IngredientServices : IIngredientServices
	{
		private IProjectDbConn _projectDbConn;

		public IngredientServices(IProjectDbConn projectDbConn)
		{
			this._projectDbConn = projectDbConn;
		}

		public async Task<bool> InsertIngredient(AddNewIngredient ingredient, int uid)
		{
			var SPName = "\"ProvisionProj\".insertIngredient";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("image_id", ingredient.Image);
			parameters.Add("t", ingredient.Title);
			parameters.Add("d", ingredient.Details);
			parameters.Add("u", ingredient.Unit);
			parameters.Add("price", ingredient.PricePerUnit);
			parameters.Add("uid", uid);

			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}


		public async Task<bool> UpdateIngredient(UpdateIngredient ingredient)
		{
			var SPName = "\"ProvisionProj\".updateIngredient";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("id", ingredient.Id);
			parameters.Add("image_id", ingredient.Image);
			parameters.Add("t", ingredient.Title);
			parameters.Add("d", ingredient.Details);
			parameters.Add("u", ingredient.Unit);
			parameters.Add("price", ingredient.PricePerUnit);

			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}


		public async Task<List<Ingredient>> GetIngredients()
		{
			var SPName = "\"ProvisionProj\".getIngredients";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			var queryResult = await this._projectDbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var ingredient = DataTableToObject.ConvertDataTable<Ingredient>(queryResult);
				return ingredient;
			}
			return null;
		}
	}
}
