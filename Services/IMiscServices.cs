using ProvisionAPI.Models;

namespace ProvisionAPI.Services
{
	public interface IMiscServices
	{
		public Task<bool> InsertNewUnit(string Title);
		public Task<bool> UpdateUnit(Unit unit);
		public Task<bool> RemoveUnit(int UnitId);
		public Task<List<Unit>> GetUnits();

	}
}