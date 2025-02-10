using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace ProvisionAPI.Services
{
	public class ProjectDbConn : IProjectDbConn
	{
		//https://github.com/npgsql/npgsql
		private NpgsqlDataSource _dataSource;
		private NpgsqlConnection _connection;
		//open new connection every new request 
		public ProjectDbConn(string connectionString)
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
			_dataSource = dataSourceBuilder.Build();
			_connection = _dataSource.OpenConnectionAsync().Result;
		}
		private DbType getDbType(object val)
		{
			if (val.GetType() == typeof(DateTime))
			{
				return DbType.DateTime;
			}
			return DbType.String; //default string
		}
		private NpgsqlDbType getNpgsqlDbType(object val)
		{
			if (val.GetType() == typeof(DateTime))
			{
				return NpgsqlDbType.Timestamp;
			}
			if(val.GetType() == typeof(int))
			{
				return NpgsqlDbType.Integer;
			}
			if (val.GetType() == typeof(bool))
			{
				return NpgsqlDbType.Boolean;
			}

			return NpgsqlDbType.Varchar; //default string
		}

		public async Task<bool> InsertByQuery(string TableName, IDictionary<string, object> QueryParameters)
		{
			var columnNames = QueryParameters.Keys.Select(x => string.Format("(@{0})", x)).ToList();
			var query = string.Format("INSERT INTO table ({0}) VALUES {1}", TableName, string.Join(",", columnNames));

			await using var cmd = new NpgsqlCommand(query, _connection);
			foreach (var parameter in QueryParameters)
			{
				cmd.Parameters.Add(new NpgsqlParameter(parameter.Key, parameter.Value)
				{
					DbType = getDbType(parameter.Value)
				});
			}

			var result = await cmd.ExecuteNonQueryAsync();
			return result > 1; //return if row affected > 1
		}
		public async Task<DataTable> ExecuteQuery(string Query)
		{
			await using var command = new NpgsqlCommand(Query, _connection);
			await using var reader = await command.ExecuteReaderAsync();

			DataTable dt = new DataTable();
			dt.Clear(); //clear it all 

			var column = reader.GetColumnSchema();
			foreach (var col in column)
			{
				var newCol = new DataColumn();
				newCol.ColumnName = col.ColumnName;
				newCol.DataType = col.DataType;
				dt.Columns.Add(newCol);
			}
			while (await reader.ReadAsync())
			{
				DataRow row = dt.NewRow();
				foreach (var col in column)
				{
					row[col.ColumnName] = reader.GetValue(col.ColumnOrdinal.Value);

				}
				dt.Rows.Add(row);
				//Console.WriteLine(reader.GetString(0));
			}
			return dt;
		}


		public async Task<bool> ExecuteStoredProc(string SPName, IDictionary<string, object> parameters)
		{
			//since SP cannot return anything, so better to return bool
			try
			{

				using var command1 = new NpgsqlCommand(SPName, _connection)
				{
					CommandType = CommandType.StoredProcedure
				};

				foreach (var param in parameters)
				{
					command1.Parameters.Add(new NpgsqlParameter() { Value = param.Value, NpgsqlDbType = getNpgsqlDbType(param.Value), ParameterName = param.Key, Direction = ParameterDirection.Input });
				}

				await using var reader = await command1.ExecuteReaderAsync();

				return true;

				//DataTable dt = new DataTable();
				//dt.Clear(); //clear it all 

				//var column = reader.GetColumnSchema();
				//foreach (var col in column)
				//{
				//	dt.Columns.Add(col.ColumnName);
				//}
				//while (await reader.ReadAsync())
				//{
				//	DataRow row = dt.NewRow();
				//	foreach (var col in column)
				//	{
				//		row[col.ColumnName] = reader.GetValue(col.ColumnOrdinal.Value);
				//	}
				//	dt.Rows.Add(row);
				//	//Console.WriteLine(reader.GetString(0));
				//}
				//return dt;
			}
			catch (Exception ex)
			{
				throw;
			}

		}

		public async Task<DataTable> ExecuteFunctions(string FunctionName, IDictionary<string, object> parameters)
		{
			var query = string.Format("SELECT * FROM {0}({1})", FunctionName, GenerateFunctionParameters(parameters));
			return await ExecuteQuery(query);
		}

		private string GenerateFunctionParameters(IDictionary<string, object> parameters)
		{
			var query = "";
			var queryList = new List<string>();
			foreach (var param in parameters.Values)
			{
				var val = string.Empty;
				if (param is string)
				{
					val = string.Format("'{0}'", param);
				}
				if (param is int || param is decimal || param is double)
				{
					val = param.ToString();
				}
				if (param == null)
				{
					val = "null";
				}

				if (!string.IsNullOrEmpty(val))
				{
					queryList.Add(val);
				}
			}
			query = string.Join(",", queryList.ToArray());
			return query;
		}
	}
}
