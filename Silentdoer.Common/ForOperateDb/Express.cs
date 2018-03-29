using System;
using System.Runtime.Serialization;

namespace Silentdoer.Common
{
	[DataContract]
	[Serializable]
	public enum Express
	{
		/// <summary>
		/// 等于
		/// </summary> 
		[EnumMember]
		Equals,
		/// <summary>
		/// 不等于
		/// </summary>
		[EnumMember]
		NotEquals,
		/// <summary>
		/// 大于
		/// </summary>
		[EnumMember]
		Greater,
		/// <summary>
		/// 大于等于
		/// </summary>
		[EnumMember]
		GreaterOrEquals,
		/// <summary>
		/// 小于
		/// </summary>
		[EnumMember]
		Less,
		/// <summary>
		/// 小于等于
		/// </summary>
		[EnumMember]
		LessOrEquals,
		/// <summary>
		/// 以...开头
		/// </summary>
		[EnumMember]
		StartWith,
		/// <summary>
		/// 以...结尾
		/// </summary>
		[EnumMember]
		EndWith,
		/// <summary>
		/// 包含
		/// </summary>
		[EnumMember]
		Like,
		/// <summary>
		/// 不包含
		/// </summary>
		[EnumMember]
		NotLike,
		/// <summary>
		/// 在集合内
		/// </summary>
		[EnumMember]
		In,
		/// <summary>
		/// 不在集合内
		/// </summary>
		[EnumMember]
		NotIn,
		/// <summary>
		/// 在区间内（包括两端端值）
		/// </summary>
		[EnumMember]
		Between,
		/// <summary>
		/// 表示未定义的Express类型
		/// </summary>
		[EnumMember]
		Unknown
	}

	public static class ExpressHelper
	{
		public static Express GetExpressEnum(string exp)
		{
			if(exp == "=")
			{
				return Express.Equals;
			}
			if(exp == "<")
			{
				return Express.Less;
			}
			if(exp == "<=")
			{
				return Express.LessOrEquals;
			}
			if(exp == ">")
			{
				return Express.Greater;
			}
			if(exp == ">=")
			{
				return Express.GreaterOrEquals;
			}
			if(exp == "!=")
			{
				return Express.NotEquals;
			}
			if(exp == "Like")
			{
				return Express.Like;
			}
			if(exp == "NotLike")
			{
				return Express.NotLike;
			}
			if(exp == "StartWith")
			{
				return Express.StartWith;
			}
			if(exp == "EndWith")
			{
				return Express.EndWith;
			}
			if(exp == "In")
			{
				return Express.In;
			}
			if(exp == "NotIn")
			{
				return Express.NotIn;
			}
			if(exp == "Between")
			{
				return Express.Between;
			}
			return Express.Unknown;
		}

		public static string GetExpressString(Express exp)
		{
			switch(exp)
			{
				case Express.Equals:
					return "=";
				case Express.Less:
					return "<";
				case Express.LessOrEquals:
					return "<=";
				case Express.Greater:
					return ">";
				case Express.GreaterOrEquals:
					return ">=";
				case Express.NotEquals:
					return "!=";
				case Express.Like:
					return "Like";
				case Express.NotLike:
					return "NotLike";
				case Express.StartWith:
					return "StartWith";
				case Express.EndWith:
					return "EndWith";
				case Express.In:
					return "In";
				case Express.NotIn:
					return "NotIn";
				case Express.Between:
					return "Between";
				case Express.Unknown:
				default:
					return "Unknown";
			}
		}
	}
}
