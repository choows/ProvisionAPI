using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.IngredientController
{
	public class AddNewIngredient
	{
		public long Image { get; set; }
		[NotNull]
		public string Title { get; set; }
		public string Details { get; set; }
		[NotNull]
		public int Unit { get; set; }
		[NotNull]
		public double PricePerUnit { get; set; }
	}
}
