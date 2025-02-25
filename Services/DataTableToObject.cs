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
							//have to declare the type one by one 
							switch (pro.PropertyType)
							{
								case Type type when type == typeof(int):
									pro.SetValue(obj, dr.Field<int>(column.ColumnName), null);
									break;
								case Type type when type == typeof(string):
									pro.SetValue(obj, dr.Field<string>(column.ColumnName), null);
									break;
								case Type type when type == typeof(DateTime):
									pro.SetValue(obj, dr.Field<DateTime>(column.ColumnName), null);
									break;
								case Type type when type == typeof(bool):
									pro.SetValue(obj, dr.Field<bool>(column.ColumnName), null);
									break;
								case Type type when type == typeof(decimal):
									pro.SetValue(obj, dr.Field<decimal>(column.ColumnName), null);
									break;
								case Type type when type == typeof(double):
									pro.SetValue(obj, dr.Field<double>(column.ColumnName), null);
									break;
								case Type type when type == typeof(DateTime?):
									pro.SetValue(obj, dr.Field<DateTime?>(column.ColumnName), null);
									break;
							}
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
