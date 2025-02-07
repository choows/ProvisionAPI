using System.Data;

namespace ProvisionAPI.Services
{
	public interface IProjectDbConn
	{
		public Task<bool> InsertByQuery(string TableName, IDictionary<string, object> Parameters);
		public Task<DataTable> ExecuteQuery(string Query);
		public Task<DataTable> ExecuteStoredProc(string SPName, IDictionary<string, object> parameters);
		public Task<DataTable> ExecuteFunctions(string FunctionName, IDictionary<string, object> parameters);
	}
}
