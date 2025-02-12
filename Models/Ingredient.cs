namespace ProvisionAPI.Models
{
	public class Ingredient
	{
		public int ID { get; set; }
		public long Image { get; set; }
		public string Title { get; set; }
		public string Details { get; set; }
		public int Unit { get; set; }
		public double PricePerUnit { get; set; }
		public int CreatedBy { get; set; }
	}
}
