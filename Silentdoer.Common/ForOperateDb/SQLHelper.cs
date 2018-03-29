using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silentdoer.Common
{
	internal static class SQLHelper
	{
		/// <summary>
		/// 将filters解析成如：delete from TbName where A="a" && B="b" 的A="a" && B="b"部分
		/// </summary>
		public static string ParseFiltersToWherePartExpressionString(List<FilterInfo> filters)
		{
			if(filters == null || filters.Count < 1)
				throw new ArgumentException("filters参数不正确。");
			var body = new StringBuilder(512);
			body.Append(ParseFilterToWhereCell(filters[0]));
			for(int i = 1; i < filters.Count; i++)
			{
				body.Append($"{RelationshipHelper.GetRelationshipString(filters[i - 1].Relationship)}{ParseFilterToWhereCell(filters[i])}");
			}
			return body.ToString();
		}

		private static string ParseFilterToWhereCell(FilterInfo filter)
		{
			// 自我规定，MySQL不允许以比较符比较null，即 ClsId > null 在我这不被允许。（虽然这在MySQL里不报错，但没意义）
			// 还有就是bool类型只允许比较是否相等，比较大小目前没意义。
			if(filter.Express == Express.Unknown)
				throw new ArgumentException("filter.Express不正确。");
			if(filter.Value is byte[])
				throw new ArgumentException("byte[]列的值不支持转换为SQL。");
			string rst = string.Empty;
			if(filter.Express == Express.Equals)
			{
				if(filter.Value == DBNull.Value || filter.Value == null)
				{
					rst = $"{filter.Field} is null";
				} else if(filter.Value is bool)
				{
					rst = $"{filter.Field}={Convert.ToInt32(filter.Value)}";
				} else
				{
					rst = $"{filter.Field}=\"{filter.Value}\"";
				}
				return rst;
			} else if(filter.Express == Express.NotEquals)
			{
				if(filter.Value == DBNull.Value || filter.Value == null)
				{
					rst = $"{filter.Field} is not null";
				} else if(filter.Value is bool)
				{
					rst = $"{filter.Field}!={Convert.ToInt32(filter.Value)}";
				} else
				{
					rst = $"{filter.Field}!=\"{filter.Value}\"";
				}
				return rst;
			} else  // 其它比较方式。
			{
				// 除了等于和不等于，其它类型的比较对于bool和null在我这都是不合规定的。
				if(filter.Value == DBNull.Value || filter.Value == null)
				{
					// ClsName > null 貌似有时是有意义的，以后看情况更新吧。
					throw new ArgumentException("参数组合后无意义不合规定。");
				}
				if(filter.Value is bool)
				{
					throw new ArgumentException("参数组合后无意义不合规定。");
				}
			}
			switch(filter.Express)
			{
				case Express.Greater:
					rst = $"{filter.Field}>\"{filter.Value}\"";
					break;
				case Express.GreaterOrEquals:
					rst = $"{filter.Field}>=\"{filter.Value}\"";
					break;
				case Express.Less:
					rst = $"{filter.Field}<\"{filter.Value}\"";
					break;
				case Express.LessOrEquals:
					rst = $"{filter.Field}<=\"{filter.Value}\"";
					break;
				case Express.In:
					var valueTypeIn = filter.Value.GetType();
					// Value 是数组
					if(valueTypeIn.IsArray)
					{
						var builder = new StringBuilder(128);
						var arr = (Array)filter.Value;
						if(arr.Length < 1)
						{
							throw new ArgumentException("Express.In对应的Value不正确。");
						}
						var tmp = arr.GetValue(0);
						builder.Append($"{filter.Field} in (\"{tmp}\"");  // ClsName in ('30'
						for(int i = 1; i < arr.Length; i++)
						{
							tmp = arr.GetValue(i);
							builder.Append($",\"{tmp}\"");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					} else if(valueTypeIn.IsGenericType) // 是List集合
					{
						var builder = new StringBuilder(128);
						var list = (IList)filter.Value;
						if(list.Count < 1)
						{
							throw new ArgumentException("Express.In对应的Value不正确。");
						}
						var tmp = list[0];
						builder.Append($"{filter.Field} in (\"{tmp}\"");  // ClsName in ('30'
						for(int i = 1; i < list.Count; i++)
						{
							tmp = list[i];
							builder.Append($",\"{tmp}\"");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					} else // 字符串形式，只允许"30","40"或('30',"40","50")，但是允许插入空格
					{
						var builder = new StringBuilder(128);
						var strArr = filter.Value.ToString().Replace("(", "").Replace(")", "").Split(',');
						if(strArr.Length < 1)
						{
							throw new ArgumentException("Express.In对应的Value格式不正确，最佳格式为(\"aValue\", \"bValue\")");
						}
						var tmp = strArr[0].Trim();  //'30'
						builder.Append($"{filter.Field} in ({tmp}");  // ClsName in ('30'
						for(int i = 1; i < strArr.Length; i++)
						{
							tmp = strArr[i].Trim();
							builder.Append($",{tmp}");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					}
					break;
				case Express.NotIn:
					valueTypeIn = filter.Value.GetType();
					// Value 是数组
					if(valueTypeIn.IsArray)
					{
						var builder = new StringBuilder(128);
						var arr = (Array)filter.Value;
						if(arr.Length < 1)
						{
							throw new ArgumentException("Express.NotIn对应的Value不正确。");
						}
						var tmp = arr.GetValue(0);
						builder.Append($"{filter.Field} not in (\"{tmp}\"");  // ClsName in ('30'
						for(int i = 1; i < arr.Length; i++)
						{
							tmp = arr.GetValue(i);
							builder.Append($",\"{tmp}\"");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					} else if(valueTypeIn.IsGenericType) // 是List集合
					{
						var builder = new StringBuilder(128);
						var list = (IList)filter.Value;
						if(list.Count < 1)
						{
							throw new ArgumentException("Express.NotIn对应的Value不正确。");
						}
						var tmp = list[0];
						builder.Append($"{filter.Field} not in (\"{tmp}\"");  // ClsName in ('30'
						for(int i = 1; i < list.Count; i++)
						{
							tmp = list[i];
							builder.Append($",\"{tmp}\"");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					} else // 字符串形式，只允许 '30','40'或('30',"40", '5 0')，但是允许插入空格。
					{
						var builder = new StringBuilder(128);
						var strArr = filter.Value.ToString().Replace("(", "").Replace(")", "").Split(',');
						if(strArr.Length < 1)
						{
							throw new ArgumentException("Express.NotIn对应的Value格式不正确，最佳格式为(\"aValue\", \"bValue\")");
						}
						var tmp = strArr[0].Trim();  //'30'
						builder.Append($"{filter.Field} not in ({tmp}");  // ClsName not in ('30'
						for(int i = 1; i < strArr.Length; i++)
						{
							tmp = strArr[i].Trim();
							builder.Append($",{tmp}");  // ClsName in ('30','40'
						}
						builder.Append(")");  // ClsName in ('30','40','50')
						rst = builder.ToString();
					}
					break;
				case Express.StartWith:
					throw new ArgumentException("MySql中暂时未实现这个功能。");
				case Express.EndWith:
					throw new ArgumentException("MySql中暂时未实现这个功能。");
				case Express.Between:
					var valueType = filter.Value.GetType();
					// Value 是数组
					if(valueType.IsArray)
					{
						var arr = (Array)filter.Value;
						if(arr.Length != 2)
						{
							throw new ArgumentException("Express.Between对应的Value不正确。");
						}
						var left = arr.GetValue(0);
						var right = arr.GetValue(1);
						rst = $"{filter.Field} between \"{left}\" and \"{right}\"";
					} else if(valueType.IsGenericType) // 是List集合
					{
						var list = (IList)filter.Value;
						if(list.Count != 2)
						{
							throw new ArgumentException("Express.Between对应的Value不正确。");
						}
						var left = list[0];
						var right = list[1];
						rst = $"{filter.Field} between \"{left}\" and \"{right}\"";
					} else // 字符串形式，只允许'30', '40'或('30','40')，或("30","40"),允许插入空格。( '30',    '40')可以。
					{
						var strList = filter.Value.ToString().Split(',');
						if(strList.Length != 2)
						{
							throw new ArgumentException("Express.Between对应的Value格式不正确，最佳格式为(\"aValue\", \"bValue\")");
						} else
						{
							var left = strList[0].Replace("(", "").Trim();  //'30'
							var right = strList[1].Replace(")", "").Trim();
							rst = $"{filter.Field} between {left} and {right}";
						}
					}
					break;
				case Express.Like:
					rst = $"{filter.Field} like \"{filter.Value}\"";
					break;
				case Express.NotLike:
					rst = $"{filter.Field} not like \"{filter.Value}\"";
					break;
			}
			return rst;
		}

		/// <summary>
		/// 将pairs解析成如：update TbName set A="a",B="b" 的A="a",B="b"部分。
		/// </summary>
		public static string ParsePairsToSetPartExpressionString(List<FieldValuePair> pairs)
		{
			if(pairs == null || pairs.Count < 1)
				throw new ArgumentException("pairs参数不正确。");
			return string.Join(",", pairs.Select(itm => $"{itm.Field}={GetOwnStrValueToAssignmentByType(itm.Value)}"));
		}

		/// <summary>
		/// 将pairs解析成如：insert into TbName(A,B,C) values("a","b","c"); 的(A,B,C) values("a","b","c")部分。
		/// </summary>
		public static string ParsePairsToInsertIntoPartExpressionString(List<FieldValuePair> pairs)
		{
			if(pairs == null || pairs.Count < 1)
				throw new ArgumentException("pairs参数不正确。");
			var fieldPart = $"({string.Join(",", pairs.Select(i => i.Field))})";  // (A,B,C)
			var valuePart = $"({string.Join(",", pairs.Select(itm => GetOwnStrValueToAssignmentByType(itm.Value)))})";  // ("a",null,1)
			return $"{fieldPart} values{valuePart}";  // (A,B,C) values("a",null,1)
		}

		/// <summary>
		/// 将pairs解析成如：insert into TbName(A,B,C) values("a","b","c"),(null,"u","d"); 的(A,B,C) values("a","b","c"),(null,"u","d")部分。
		/// </summary>
		public static string ParseValuesPairsToInsertIntoPartExpressionString(List<FieldValuesPair> valuesPairs)
		{
			if(valuesPairs == null || valuesPairs.Count < 1)
				throw new ArgumentException("valuesPairs参数不正确。");
			var fieldPart = $"({string.Join(",", valuesPairs.Select(i => i.Field))})";  // (A,B,C)
																						// 第一列的(A列) 值集
																						// 要ToList()，否则编译器会做一些操作，这个要等到生成List时才会真正执行。
			var seed = valuesPairs[0].Values.Select(GetOwnStrValueToAssignmentByType);  // IEnumerable<string> {"a",null}
																						// ("a",1,null),(null,"hao",null)
			var valuesPart = string.Join(",", valuesPairs.Skip(1).Aggregate(seed, (bodyWithSeed, nextStartsFirst) =>
					  bodyWithSeed.Zip(nextStartsFirst.Values, (l, r) => string.Join(",", l, GetOwnStrValueToAssignmentByType(r)))).Select(i => $"({i})"));
			return $"{fieldPart} values{valuesPart}";
		}

		// 只能用于SQL中的赋值部分，如set A="a";或insert into .....values("a"
		private static string GetOwnStrValueToAssignmentByType(object value)
		{
			if(value is byte[])  // C# null is byte[]判断为false。
				throw new ArgumentException("byte[]类型的列值不支持转换为SQL语句。");
			string rst;
			if(value == DBNull.Value || value == null)
			{
				// "null"
				rst = "null";
			} else if(value is bool)
			{
				// "1" or "0"
				rst = Convert.ToInt32(value).ToString();
			} else
			{
				// ""Wlq""
				rst = $"\"{value}\"";
			}
			return rst;
		}
	}
}
