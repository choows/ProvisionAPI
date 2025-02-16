using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.BucketController
{
	public class UpdateBucket
	{
		[NotNull]
		public int BucketId { get; set; }
		public List<IngredientUpdate>? Ingredients { get; set; }
		public class IngredientUpdate {
            public int IngredientId { get; set; }
			public double Required { get; set; }
            public double Purchased { get; set; }
		}

	}
}
