using System;

namespace ProvisionAPI.Models
{
	public class Bucket
	{
		public DateTime PurchaseDate { get; set; }
		public List<IngredientRequired> ingredients { get; set; }
	}
	public class IngredientRequired
	{
		//public int BucketId { get; set; }
		public int IngredientId { get; set; }
		public double Required { get; set; }
		public string ImagePath { get; set; }
		public double Purchased { get; set; }
		public string Title { get; set; }
		public string Details { get; set; }
		public double PricePerUnit { get; set; }
		public string UnitTitle { get; set; }
		//public int RecipeID { get; set; }   //TODO : adding recipe into the response

	}
}
