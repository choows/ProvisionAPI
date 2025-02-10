using ProvisionAPI.Models;

namespace ProvisionAPI.Services
{
	public class MiscServices : IMiscServices
	{
		private IProjectDbConn _projectDbConn;

		public MiscServices(IProjectDbConn projectDbConn) {

			this._projectDbConn = projectDbConn;
		}

		public async Task<bool> InsertNewUnit(string Title)
		{
			var SPName = "\"ProvisionProj\".insertUnit";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			//validate
			if (string.IsNullOrEmpty(Title))
			{
				throw new Exception("Unit title not be null");
			}
			parameters.Add("t", Title);
			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}

		public async Task<bool> UpdateUnit(Unit unit)
		{
			var SPName = "\"ProvisionProj\".updateUnit";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			//validate
			if (string.IsNullOrEmpty(unit.Title))
			{
				throw new Exception("Unit shall not be null");
			}
			parameters.Add("id", unit.ID);
			parameters.Add("t", unit.Title);
			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}

		public async Task<bool> RemoveUnit(int UnitId)
		{
			var SPName = "\"ProvisionProj\".removeUnit";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("id", UnitId);
			var result = await this._projectDbConn.ExecuteStoredProc(SPName, parameters);
			return result;
		}
		public async Task<List<Unit>> GetUnits()
		{
			var SPName = "\"ProvisionProj\".getunits";
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			var queryResult = await this._projectDbConn.ExecuteFunctions(SPName, parameters);
			if (queryResult.Rows.Count > 0)
			{
				var units = DataTableToObject.ConvertDataTable<Unit>(queryResult);
				return units;
			}
			return null;
		}
	}
}
