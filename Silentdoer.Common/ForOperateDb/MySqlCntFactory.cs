using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Silentdoer.Common
{
	public class MySqlCntFactory
	{
		public static MySqlConnection CreateInstance()
		{
			return new MySqlConnection(GetConnectionStr());
		}

		public static MySqlConnection CreateInstance(string connectionStr)
		{
			return new MySqlConnection(connectionStr);
		}

		public static string GetConnectionStr()
		{
			return ConfigurationManager.ConnectionStrings["LocalConnectionString"].ConnectionString;
		}
	}
}
