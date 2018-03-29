/*--------------------------------------------------------------------------------------------------------------------
 * Author:		silentdoer
 * Date:		2017-2-13
 * Function:		Dal通用类，通过此类可以操作同一Schema的任一数据表。
 -------------------------------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Silentdoer.Common
{
	public static class DalManager
	{
		/// <summary>
		/// 插入一条记录，pairsForInsert中有插入此条数据所需的列值，成功返回true
		/// </summary>
		public static bool InsertRecord(Func<string> getDbTableName, List<FieldValuePair> pairsForInsert)
		{
			var insertIntoPart = SQLHelper.ParsePairsToInsertIntoPartExpressionString(pairsForInsert);
			var strCmd = $"insert into {getDbTableName()}{insertIntoPart};";
			return MySqlHelper.ExecuteCmd(strCmd) == 1;
		}

		/// <summary>
		/// 插入多条记录，valuesPairsForInsert中有插入此条数据所需的列值，返回成功插入的行数。
		/// 请保证每个FieldValuesPair对象(代表列)的Values(代表列值集)的长度一样，否则将只插入 最“短”列值集的长度的行数。
		/// </summary>
		public static int InsertRecords(Func<string> getDbTableName, List<FieldValuesPair> valuesPairsForInsert)
		{
			var insertIntoPart = SQLHelper.ParseValuesPairsToInsertIntoPartExpressionString(valuesPairsForInsert);
			var strCmd = $"insert into {getDbTableName()}{insertIntoPart};";
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// 通过等值比较某列来删除表中数据。
		/// </summary>
		public static int DeleteRecordsByEqualsField(Func<string> getDbTableName, string filed, object value)
		{
			var filters = new List<FilterInfo> { new FilterInfo(filed, value) };
			return DeleteRecordsByFilters(getDbTableName, filters);
		}

		/// <summary>
		/// 通过单个filter删除表中数据。
		/// </summary>
		public static int DeleteRecordsByFilter(Func<string> getDbTableName, FilterInfo filterForWhere)
		{
			var filters = new List<FilterInfo> { filterForWhere };
			return DeleteRecordsByFilters(getDbTableName, filters);
		}

		/// <summary>
		/// 通过filters删除表中数据。
		/// </summary>
		public static int DeleteRecordsByFilters(Func<string> getDbTableName, List<FilterInfo> filtersForWhere)
		{
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			var strCmd = $"delete from {getDbTableName()} where {wherePart};";
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// delete表中所有的数据，慎用。
		/// </summary>
		public static int DeleteAllRecords(Func<string> getDbTableName)
		{
			var strCmd = $"delete from {getDbTableName()};";
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// 获取符合此等值比较条件的记录条数。
		/// </summary>
		public static int GetRecordsCountByEqualsField(Func<string> getDbTableName,
			string field, object value)
		{
			var filters = new List<FilterInfo> { new FilterInfo(field, value) };
			return GetRecordsCountByFilters(getDbTableName, filters);
		}

		/// <summary>
		/// 获取符合此filter条件的记录条数。
		/// </summary>
		public static int GetRecordsCountByFilter(Func<string> getDbTableName,
			FilterInfo filterForWhere)
		{
			var filters = new List<FilterInfo> { filterForWhere };
			return GetRecordsCountByFilters(getDbTableName, filters);
		}

		/// <summary>
		/// 获取符合此filters条件的记录条数。
		/// </summary>
		public static int GetRecordsCountByFilters(Func<string> getDbTableName,
			List<FilterInfo> filtersForWhere)
		{
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			return MySqlHelper.CountOfQuery(getDbTableName(), wherePart);
		}

		/// <summary>
		/// 获取此数据表中所有的记录的条数。
		/// </summary>
		public static int GetAllRecordsCount(Func<string> getDbTableName)
		{
			// "1"表示 where 1
			return MySqlHelper.CountOfQuery(getDbTableName(), "1");
		}

		/// <summary>
		/// 通过等值比较某一列来获取结果集。
		/// </summary>
		public static List<TResult> GetRecordsByEqualsField<TResult>(Func<string> getDbTableName,
			string field, object value, Func<DataTable, List<TResult>> convert)
		{
			var filters = new List<FilterInfo> { new FilterInfo(field, value) };
			return GetRecordsByFilters(getDbTableName, filters, convert);
		}

		/// <summary>
		/// 通过等值比较某一列获取表的部分字段所组成的新“表类”的结果集，convert是新“表类”的convert，getDbTableName是数据库中的表/视图名。
		/// </summary>
		public static List<TResult> GetRecordsOfTablePartByEqualsField<TResult>(Func<string> getDbTableName, string[] fields,
			string compareField, object compareValue, Func<DataTable, List<TResult>> convert)
		{
			var filters = new List<FilterInfo> { new FilterInfo(compareField, compareValue) };
			return GetRecordsOfTablePartByFilters(getDbTableName, fields, filters, convert);
		}

		/// <summary>
		/// 通过单个过滤对象来获取结果集
		/// </summary>
		public static List<TResult> GetRecordsByFilter<TResult>(Func<string> getDbTableName,
			FilterInfo filterForWhere, Func<DataTable, List<TResult>> convert)
		{
			var filters = new List<FilterInfo> { filterForWhere };
			return GetRecordsByFilters(getDbTableName, filters, convert);
		}

		/// <summary>
		/// 通过单个过滤对象获取表的部分字段所组成的新“表类”的结果集，convert是新“表类”的convert，getDbTableName是数据库中的表/视图名。
		/// </summary>
		public static List<TResult> GetRecordsOfTablePartByFilter<TResult>(Func<string> getDbTableName, string[] fields,
			FilterInfo filterForWhere, Func<DataTable, List<TResult>> convert)
		{
			var filters = new List<FilterInfo> { filterForWhere };
			return GetRecordsOfTablePartByFilters(getDbTableName, fields, filters, convert);
		}

		/// <summary>
		/// 通过多个过滤对象来获取记录集
		/// </summary>
		public static List<TResult> GetRecordsByFilters<TResult>(Func<string> getDbTableName,
			List<FilterInfo> filtersForWhere, Func<DataTable, List<TResult>> convert)
		{
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			var strCmd = $"select* from {getDbTableName()} where {wherePart};";
			var dt = MySqlHelper.Query(strCmd);
			return convert(dt);
		}

		/// <summary>
		/// 通过多个过滤对象获取表的部分字段所组成的新“表类”的结果集，convert是新“表类”的convert，getDbTableName是数据库中的表/视图名。
		/// </summary>
		public static List<TResult> GetRecordsOfTablePartByFilters<TResult>(Func<string> getDbTableName, string[] fields,
			List<FilterInfo> filtersForWhere, Func<DataTable, List<TResult>> convert)
		{
			var selectPart = string.Join(",", fields);
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			var strCmd = $"select {selectPart} from {getDbTableName()} where {wherePart};";
			var dt = MySqlHelper.Query(strCmd);
			return convert(dt);
		}

		/// <summary>
		/// 获取所有的记录，慎用。
		/// </summary>
		public static List<TResult> GetAllRecords<TResult>(Func<string> getDbTableName, Func<DataTable, List<TResult>> convert)
		{
			var strCmd = $"select* from {getDbTableName()};";
			var dt = MySqlHelper.Query(strCmd);
			return convert(dt);
		}

		/// <summary>
		/// 获取表的部分字段所组成的新“表类”的所有的记录，convert是新“表类”的convert，getDbTableName是数据库中的表/视图名。
		/// </summary>
		public static List<TResult> GetAllRecordsOfTablePart<TResult>(Func<string> getDbTableName, string[] fields, Func<DataTable, List<TResult>> convert)
		{
			var selectPart = string.Join(",", fields);
			var strCmd = $"select {selectPart} from {getDbTableName()};";
			var dt = MySqlHelper.Query(strCmd);
			return convert(dt);
		}

		/// <summary>
		/// 通过pair和filter更新符合条件的记录
		/// getDbTableName是Entity.GetDbTableName；若MySQL执行strCmd语句出错，则返回负数（一般是-1）
		/// </summary>
		public static int UpdateRecordsByPairFilter(Func<string> getDbTableName, FieldValuePair pairForSet, FilterInfo filterForWhere)
		{
			var pairs = new List<FieldValuePair> { pairForSet };
			var filters = new List<FilterInfo> { filterForWhere };
			return UpdateRecordsByPairsFilters(getDbTableName, pairs, filters);
		}

		/// <summary>
		/// 通过pairs和filter更新符合条件的记录
		/// getDbTableName是Entity.GetDbTableName；若MySQL执行strCmd语句出错，则返回负数（一般是-1）
		/// </summary>
		public static int UpdateRecordsByPairsFilter(Func<string> getDbTableName, List<FieldValuePair> pairsForSet, FilterInfo filterForWhere)
		{
			var filters = new List<FilterInfo> { filterForWhere };
			return UpdateRecordsByPairsFilters(getDbTableName, pairsForSet, filters);
		}

		/// <summary>
		/// 通过pair和filters更新符合条件的记录
		/// getDbTableName是Entity.GetDbTableName；若MySQL执行strCmd语句出错，则返回负数（一般是-1）
		/// </summary>
		public static int UpdateRecordsByPairFilters(Func<string> getDbTableName, FieldValuePair pairForSet, List<FilterInfo> filtersForWhere)
		{
			var pairs = new List<FieldValuePair> { pairForSet };
			return UpdateRecordsByPairsFilters(getDbTableName, pairs, filtersForWhere);
		}

		/// <summary>
		/// 通过pairs和filters更新符合条件的记录
		/// getDbTableName是Entity.GetDbTableName；若MySQL执行strCmd语句出错，则返回负数（一般是-1）
		/// </summary>
		public static int UpdateRecordsByPairsFilters(Func<string> getDbTableName, List<FieldValuePair> pairsForSet, List<FilterInfo> filtersForWhere)
		{
			var setPart = SQLHelper.ParsePairsToSetPartExpressionString(pairsForSet);
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			// "update Student set Name = 'WLQ', ClsName = '高三三班' where StuId=2;"
			var strCmd = $"update {getDbTableName()} set {setPart} where {wherePart};";
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// pair更新所有记录
		/// </summary>
		public static int UpdateAllRecordsByPair(Func<string> getDbTableName, FieldValuePair pairForSet)
		{
			var pairs = new List<FieldValuePair> { pairForSet };
			return UpdateAllRecordsByPairs(getDbTableName, pairs);
		}

		/// <summary>
		/// pairs更新所有记录
		/// </summary>
		public static int UpdateAllRecordsByPairs(Func<string> getDbTableName, List<FieldValuePair> pairsForSet)
		{
			var setPart = SQLHelper.ParsePairsToSetPartExpressionString(pairsForSet);
			var strCmd = $"update {getDbTableName()} set {setPart};";
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// 直接通过SQL语句执行查询操作，注意convert所属的实体类不要弄错了。
		/// </summary>
		public static List<TResult> QueryByCmd<TResult>(string strCmd, Func<DataTable, List<TResult>> convert)
		{
			var dt = MySqlHelper.Query(strCmd);
			return convert(dt);
		}

		/// <summary>
		/// 某些特殊情况直接输入sql语句执行。返回受影响的行数，负数表示MySQL服务执行SQL出错。
		/// </summary>
		public static int ExecuteCmd(string strCmd)
		{
			return MySqlHelper.ExecuteCmd(strCmd);
		}

		/// <summary>
		/// 通过存储过程来获取数据表中的记录，若是获取数据表所有字段，则可将fields设置为null
		/// </summary>
		public static List<TResult> GetPageRecordsByProc<TResult>(string procName, Func<string> getDbTableName, string[] fields, List<FilterInfo> filtersForWhere, string sortClm, bool isAscSort, int pageIndex, int pageSize, Func<DataTable, List<TResult>> convert)
		{
			var selectPart = fields == null ? "*" : string.Join(",", fields);
			var wherePart = SQLHelper.ParseFiltersToWherePartExpressionString(filtersForWhere);
			var args = new IDataParameter[]
			{
				new MySqlParameter {ParameterName = "tableName", Value = getDbTableName()},
				new MySqlParameter {ParameterName = "selectPart", Value = selectPart},
				new MySqlParameter {ParameterName = "wherePart", Value = wherePart},
				new MySqlParameter {ParameterName = "sortClm", Value = sortClm},
				new MySqlParameter {ParameterName = "isAscSort", Value = Convert.ToInt32(isAscSort)},
				new MySqlParameter {ParameterName = "pageIndex", Value = pageIndex},
				new MySqlParameter {ParameterName = "pageSize", Value = pageSize}
			};
			var dt = MySqlHelper.ProcedureQuery(procName, args);
			return convert(dt);
		}
	}
}