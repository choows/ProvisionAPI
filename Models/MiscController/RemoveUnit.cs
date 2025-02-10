using System.Diagnostics.CodeAnalysis;

namespace ProvisionAPI.Models.MiscController
{
	public class RemoveUnit
	{
		[NotNull]
		public int UnitId { get; set; }
	}
}
