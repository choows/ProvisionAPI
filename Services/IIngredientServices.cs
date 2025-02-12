using ProvisionAPI.Models;
using ProvisionAPI.Models.IngredientController;

namespace ProvisionAPI.Services
{
	public interface IIngredientServices
	{
		public Task<bool> InsertIngredient(AddNewIngredient ingredient, int uid);
		public Task<bool> UpdateIngredient(UpdateIngredient ingredient);
		public Task<List<Ingredient>> GetIngredients();

	}
}