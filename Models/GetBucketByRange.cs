namespace ProvisionAPI.Models
{
	public class GetBucketByRange
	{
		public int ID { get; set; }
		public double UnitRequired { get; set; }
		public DateTime PurchaseDate { get; set; }
		public double UnitPurchased { get; set; }
		public int RecipeID { get; set; }
		public int IngredientID { get; set; }
		public string IngredientImage { get; set; }
		public string IngredientTitle { get; set; }
		public string IngredientDetails { get; set; }
		public double PricePerUnit { get; set; }
		public string UnitTitle { get; set; }
	}
}
