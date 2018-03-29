using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Silentdoer.Common
{
	internal static class MySqlHelper
	{
		/// <summary>
		/// 通过带单个或多个参数的SQL语句查询，并将结果集通过DataTable对象返回。
		/// </summary>
		public static DataTable Query(string strCmd, IDataParameter[] args)
		{
			var result = new DataTable();
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var cmd = new MySqlCommand(strCmd, cnt);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.AddRange(args);
			var adapter = new MySqlDataAdapter(cmd);
			try
			{
				cnt.Open();
				adapter.Fill(result);
			} catch(Exception ex)
			{
				throw new Exception(
					$"{ex}{Environment.NewLine}此时strCmd：{strCmd}{Environment.NewLine}cntStr：{cntStr}{Environment.NewLine}args的值为：{string.Join("#MidMark#", args.Select(i => $"srcClm{i.SourceColumn} Value：{i.Value}"))}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			return result;
		}

		/// <summary>
		/// 通过带单个参数的SQL语句查询，并将结果集通过DataTable对象返回。
		/// </summary>
		public static DataTable Query(string strCmd, IDataParameter arg)
		{
			return Query(strCmd, new[] { arg });
		}

		/// <summary>
		/// 通过SQL语句查询表，并加结果集填充至DataTable对象中返回。
		/// </summary>
		public static DataTable Query(string strCmd)
		{
			var result = new DataTable();
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var cmd = new MySqlCommand(strCmd, cnt);
			cmd.CommandType = CommandType.Text;
			var adapter = new MySqlDataAdapter(cmd);
			try
			{
				cnt.Open();
				// 如果不指定tableKey，则第一个表默认名是Table，第二个默认名是Table1，依次类推。
				adapter.Fill(result);
				// 和DateSet都是可以存多个表的数据。
				//adapter.Fill(0, Int32.MaxValue, new DataTable[5]);
			} catch(Exception ex)
			{
				throw new Exception($"{ex}{Environment.NewLine}此时strCmd：{strCmd}{Environment.NewLine}cntStr：{cntStr}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			return result;
		}

		/// <summary>
		/// 获取数据表dbTableName以wherePartString为条件的记录的条数，wherePartString格式：A="bb" && B!="c"
		/// </summary>
		public static int CountOfQuery(string dbTableName, string wherePartString)
		{
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var strCmd = $"select count(*) from {dbTableName} where {wherePartString};";
			var cmd = new MySqlCommand(strCmd, cnt);
			cmd.CommandType = CommandType.Text;
			object count;
			try
			{
				cnt.Open();
				count = cmd.ExecuteScalar();  // 获取第一行第一列的值，其余单元格的值忽略。
			} catch(Exception ex)
			{
				throw new Exception($"{ex}{Environment.NewLine}此时strCmd：{strCmd}{Environment.NewLine}cntStr：{cntStr}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			// 若cmd.ExecuteScalar()成功执行，则count一定有值。
			return Convert.ToInt32(count);
		}

		/// <summary>
		/// 通过带单个或多个参数的SQL语句执行非查询操作，返回受影响的行数。
		/// </summary>
		public static int ExecuteCmd(string strCmd, IDataParameter[] args)
		{
			int count;
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var cmd = new MySqlCommand(strCmd, cnt);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.AddRange(args);
			try
			{
				cnt.Open();
				count = cmd.ExecuteNonQuery();
			} catch(Exception ex)
			{
				throw new Exception(
					$"{ex}{Environment.NewLine}此时strCmd：{strCmd}{Environment.NewLine}cntStr：{cntStr}{Environment.NewLine}args的值为：{string.Join("#MidMark#", args.Select(i => $"srcClm{i.SourceColumn} Value：{i.Value}"))}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			return count;
		}

		/// <summary>
		/// 通过带单个参数的SQL语句执行非查询操作，返回受影响的行数。
		/// </summary>
		public static int ExecuteCmd(string strCmd, IDataParameter arg)
		{
			return ExecuteCmd(strCmd, new[] { arg });
		}

		/// <summary>
		/// 返回-1表示执行出错。不过一般如果MySQL执行C#生成的SQL出错是会直接引发cmd.ExecuteNoQuery()处异常而非返回-1；
		/// </summary>
		//private static readonly object lkExecuteCmd = new object();
		// 将多个Insert暂存起来，多了后一起执行；专门生成一个线程来执行Insert等方式或可优化。
		public static int ExecuteCmd(string strCmd)
		{
			int count;
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var cmd = new MySqlCommand(strCmd, cnt);
			cmd.CommandType = CommandType.Text;
			try
			{
				cnt.Open();
				count = cmd.ExecuteNonQuery();
			} catch(Exception ex)
			{
				throw new Exception(
					$"{ex}{Environment.NewLine}此时strCmd：{strCmd}{Environment.NewLine}cntStr：{cntStr}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			return count;
		}

		/// <summary>
		/// 通过单个或多个参数执行存储过程，并将结果集通过DataTable对象返回。
		/// </summary>
		public static DataTable ProcedureQuery(string procName, IDataParameter[] args)
		{
			var result = new DataTable();
			var cnt = MySqlCntFactory.CreateInstance();
			var cntStr = cnt.ConnectionString;
			var cmd = new MySqlCommand(procName, cnt);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddRange(args);
			var adapter = new MySqlDataAdapter(cmd);
			try
			{
				cnt.Open();
				adapter.Fill(result);
			} catch(Exception ex)
			{
				throw new Exception(
					$"{ex}{Environment.NewLine}此时procName：{procName}{Environment.NewLine}cntStr：{cntStr}{Environment.NewLine}args的值为：{string.Join("#MidMark#", args.Select(i => $"srcClm{i.SourceColumn} Value：{i.Value}"))}");
			} finally
			{
				if(cnt.State != ConnectionState.Closed)
					cnt.Close();
			}
			return result;
		}
	}
}
