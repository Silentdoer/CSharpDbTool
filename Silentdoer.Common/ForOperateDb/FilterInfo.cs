using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Silentdoer.Common
{
	public class FilterInfo
	{
		// 必须主动写出默认构造函数，因为有了其它的构造函数，系统就不会自动分配一个默认构造函数
		public FilterInfo() { }
		/// <summary>
		/// 默认Relationship值是And，Express值是Equals
		/// </summary>
		public FilterInfo(string field, object value)
		{
			Field = field;
			Value = value;
		}
		/// <summary>
		/// 默认Relationship值是And
		/// </summary>
		public FilterInfo(string field, object value, Express express)
		{
			Field = field;
			Value = value;
			Express = express;
		}
		/// <summary>
		/// 默认Express值是Equals
		/// </summary>
		public FilterInfo(string field, object value, Relationship relationship)
		{
			Field = field;
			Value = value;
			Relationship = relationship;
		}
		public FilterInfo(string field, object value, Express express, Relationship relationship)
		{
			Field = field;
			Value = value;
			Express = express;
			Relationship = relationship;
		}
		/// <summary>
		/// 表字段
		/// </summary>	
		[DataMember]
		public string Field
		{
			get;
			set;
		}
		/// <summary>
		/// 字段值；当Express是In之类时，此值可以是数组集合，也可以是"20,30"这样的字符串形式的区间
		/// 当Express是YesNoNull是，此值是"Yes"或"No"，代表IsNull或IsNotNull
		/// </summary>	
		[DataMember]
		public object Value
		{
			set;
			get;
		}

		/// <summary>
		/// 如 Express.In;Express.Like;Express.Greater;Express.Equals 等等
		/// </summary>	
		[DataMember]
		public Express Express { get; set; } = Express.Equals;

		/// <summary>
		/// Relationship.And; Relationship.Or；代表某个FilterInfo与后面一个FilterInfo的并存关系
		/// 默认是And的关系(当只有一个FilterInfo时可以省略它）
		/// </summary>	
		[DataMember]
		// 创建FilterInfo对象时首先会执行属性初始化，即先执行非构造函数的初始化代码，然后才执行构造函数的代码。
		// 这样就不会出现构造函数无论怎么传参数，这个值都是And。（对象初始化器的方式初始化属性执行顺序和构造函数一致）。
		public Relationship Relationship { get; set; } = Relationship.And;
	}
}
