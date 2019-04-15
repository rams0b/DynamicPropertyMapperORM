using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;


    public static class DBFactory
    {
        
		public static T Add<T>(T businessObject)
        {
			Type type = businessObject.GetType();
			string className = type.Name;
			string saveStoredProcName = string.Empty; 
			int ID = GetPrimaryKey<T>(className, businessObject);

			if (ID == 0)
			{
				saveStoredProcName = $"usp_{className}Insert";
			}
			else
			{
				saveStoredProcName = $"usp_{className}Update";
			}

			SqlParameter[] parameters = GetAllSqlParameter<T>(className,businessObject,ID == 0);

			T returnObject = SqlManager.Save<T>(saveStoredProcName,Translate<T>, parameters);
			return returnObject;
		}

		public static void Delete<T>(T businessObject,string proc = "")
		{
			Type type = businessObject.GetType();
			string className = type.Name;
			string saveStoredProcName = $"usp_{className}Delete";

			if(proc != string.Empty)
			{ 
				saveStoredProcName = proc;
			}
			
			int ID = GetPrimaryKey<T>(className, businessObject);

			SqlParameter[] parameters = GetAllSqlParameter<T>(className, businessObject, false);

			SqlManager.Delete<T>(saveStoredProcName, parameters);
			
			return;
		}

		public static T Get<T>(T businessObject, string proc = "")
		{
			Type type = businessObject.GetType();
			string className = type.Name;
			string saveStoredProcName = $"usp_{className}Select";
			if (proc != string.Empty)
			{
				saveStoredProcName = proc;
			}

			SqlParameter[] parameters = GetAllSqlParameter<T>(className, businessObject, false);

			T returnObject = SqlManager.Get<T>(saveStoredProcName, Translate<T>, parameters);
			return returnObject;
		}

		public static List<T> GetAll<T>(T businessObject, string procName = "")
		{
			Type type = businessObject.GetType();
			string className = type.Name;
			string saveStoredProcName = procName == string.Empty ? $"usp_{className}Select" : procName;
			int ID = GetPrimaryKey<T>(className, businessObject);

			SqlParameter[] parameters = GetAllSqlParameter<T>(className, businessObject,false);

			List<T> returnObject = SqlManager.Get<List<T>>(saveStoredProcName, TranslateAll<T>, parameters);
			return returnObject;
		}

		private static SqlParameter[] GetAllSqlParameter<T>(string className,T businessObject, bool isInsert)
		{ 
			Type type = businessObject.GetType();
			List<SqlParameter> parameterList = new List<SqlParameter>();

			foreach(PropertyInfo property in type.GetProperties())
			{
				bool canIgnore = false;
				foreach (var attribute in property.GetCustomAttributes())
				{
					if (attribute.GetType() == typeof(IgnoreDefault))
					{
						canIgnore = true;
						break;
					}
				}

				if (canIgnore)
				{
					continue;
				}

				if (property.Name == className + "ID" && isInsert)
				{ 
					continue;
				}

				string value = property.GetValue(businessObject)?.ToString();
				if(string.IsNullOrEmpty(value) || value == "0" || value == "1/01/0001 12:00:00 a.m.")
				{
					continue;
				}

				SqlParameter parameter = new SqlParameter($"@{property.Name}", value);
				if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
				{ 
					parameter.DbType = System.Data.DbType.DateTime;
				}
				parameterList.Add(parameter);
			}

			return parameterList.ToArray();
		}

		private static SqlParameter GetPrimarySqlParameter<T>(string className, T businessObject)
		{
			Type type = businessObject.GetType();
			SqlParameter parameter = null;

			foreach (PropertyInfo property in type.GetProperties())
			{
				if (property.Name == className + "ID")
				{
					string value = property.GetValue(businessObject)?.ToString();
					parameter = new SqlParameter($"@{property.Name}", value);
					break;
				}
			}

			return parameter;
		}

		private static int GetPrimaryKey<T>(string className, T businessObject)
        {
            int returnValue = 0;
            Type type = businessObject.GetType();

            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.Name == className + "ID")
                {
                    int.TryParse(p.GetValue(businessObject)?.ToString(),out returnValue);
                }
            }
			
			return returnValue;
        }

		private static T Translate<T>(this SqlDataReader reader)
		{
			
			if(reader == null || reader.IsClosed)
				return default(T);

			reader.Read();
			
			
			T returnObject = (T)Activator.CreateInstance(typeof(T));
			Type type = returnObject.GetType();

			foreach (PropertyInfo p in type.GetProperties())
			{
				bool canIgnore = false;
				foreach(var attribute in p.GetCustomAttributes())
				{ 
					if(attribute.GetType() == typeof(IgnoreDefault))
					{ 
						canIgnore = true;
						break;
					}
				}

				if(canIgnore)
				{ 
					continue;
				}

				if (string.IsNullOrEmpty(reader[p.Name]?.ToString()))
				{
					continue;
				}

	
				object changedType = p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)
				? DateTime.Parse(reader[p.Name]?.ToString())
				: Convert.ChangeType(reader[p.Name]?.ToString(), Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

				p.SetValue(returnObject, changedType, null);
			}

			return returnObject;
		}

		private static List<T> TranslateAll<T>(this SqlDataReader reader)
		{

			List<T> returnList = new List<T>();

			if (reader == null || reader.IsClosed)
				return returnList;

			while(reader.Read())
			{
				T returnObject = (T)Activator.CreateInstance(typeof(T));
				Type type = returnObject.GetType();

				foreach (PropertyInfo p in type.GetProperties())
				{
					bool canIgnore = false;
					foreach (var attribute in p.GetCustomAttributes())
					{
						if (attribute.GetType() == typeof(IgnoreDefault))
						{
							canIgnore = true;
							break;
						}
					}

					if (canIgnore)
					{
						continue;
					}

					if (string.IsNullOrEmpty(reader[p.Name]?.ToString()))
					{ 
						continue;
					} 

					object changedType = p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)
				? DateTime.Parse(reader[p.Name]?.ToString())
				: Convert.ChangeType(reader[p.Name]?.ToString(), Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

					p.SetValue(returnObject, changedType, null);
				}

				returnList.Add(returnObject);

			}

			return returnList;
		}
	}

