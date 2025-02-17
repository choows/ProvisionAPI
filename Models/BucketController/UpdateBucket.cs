using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.BucketController
{
	public class UpdateBucket
	{
		[NotNull]
		public DateTime PurchaseDate { get; set; }
		[NotNull]
		public List<IngredientUpdate> ingredients { get; set; }
	}
	public class IngredientUpdate
	{
		//public int BucketId { get; set; }
		[NotNull]
		public int IngredientId { get; set; }
		[NotNull]
		public double Required { get; set; }
		[NotNull]
		public double Purchased { get; set; }
		//public int RecipeID { get; set; }   //TODO : adding recipe into the response

	}
}
