using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.BucketController
{
	public class AddBucket
	{
		[NotNull]
		public int IngredientID { get; set; }
		[NotNull]
		public double UnitRequired { get; set; }
		[NotNull]
		public DateOnly PurchaseDate { get; set; }
		[NotNull]
		public double UnitPurchased { get; set; }
		[AllowNull]
		public int? RecipeID { get; set; }
	}
}
