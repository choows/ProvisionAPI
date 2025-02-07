using System.Data;
using System.Reflection;

namespace ProvisionAPI.Services
{
	public static class DataTableToObject
	{
		public static List<T> ConvertDataTable<T>(DataTable dt)
		{
			List<T> data = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				T item = GetItem<T>(row);
				data.Add(item);
			}
			return data;
		}

		private static T GetItem<T>(DataRow dr)
		{
			Type temp = typeof(T);
			T obj = Activator.CreateInstance<T>();

			foreach (DataColumn column in dr.Table.Columns)
			{
				foreach (PropertyInfo pro in temp.GetProperties())
				{
					if (string.Compare(pro.Name, column.ColumnName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						try
						{
							pro.SetValue(obj, dr[column.ColumnName], null);
							break;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
					else
						continue;
				}
			}
			return obj;
		}
	}
}
