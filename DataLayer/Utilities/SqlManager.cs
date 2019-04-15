using System;
using System.Configuration;
using System.Data.SqlClient;

    public static class SqlManager
    {
       private static string ConnectionString => "Data Source=.\\SQLEXPRESS;Initial Catalog=db;Persist Security Info=True;User ID=sa;Password=123";
		

	

		public static T Save<T>(string procName,
            Func<SqlDataReader, T> translator,
            params SqlParameter[] parameters)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;
                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        T elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

		public static void Delete<T>(string procName,
		   SqlParameter[] parameters)
		{

			using (var sqlConnection = new SqlConnection(ConnectionString))
			{
				using (var sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					sqlCommand.CommandText = procName;
					if (parameters != null)
					{
						sqlCommand.Parameters.AddRange(parameters);
					}
					sqlConnection.Open();
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public static T Get<T>(string procName,
		   Func<SqlDataReader, T> translator,
		  params SqlParameter[] parameters)
		{
			using (var sqlConnection = new SqlConnection(ConnectionString))
			{
				using (var sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					sqlCommand.CommandText = procName;
					if (parameters != null)
					{
						sqlCommand.Parameters.AddRange(parameters);
					}
					sqlConnection.Open();
					using (var reader = sqlCommand.ExecuteReader())
					{
						T elements;
						try
						{
							elements = translator(reader); 
						}
						finally
						{
							while (reader.NextResult())
							{ }
						}
						return elements;
					}
				}
			}
		}


    }
