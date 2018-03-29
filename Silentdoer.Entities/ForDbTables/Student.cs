using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Silentdoer.Entities
{
	/// <summary>
	/// Student表的注释，如果没有注释，则获取注释是获取的是Student
	/// </summary>
	[DataContract]
	[Serializable]
	public class Student
	{
		#region 构造方法

		/// <summary>
		/// 默认构造方法
		/// </summary>
		public Student() { }

		/// <summary>
		/// 构造方法二，初始化主键
		/// </summary>
		public Student(long stuId)
		{
			StuId = stuId;
		}

		/// <summary>
		/// 构造方法三，初始化所有列
		/// </summary>
		public Student(long stuId, string name, string clsName, DateTime createTime, DateTime updateTime)
		{
			StuId = stuId;
			Name = name;
			ClsName = clsName;
			CreateTime = createTime;
			UpdateTime = updateTime;
		}

		#endregion 构造方法

		#region 属性

		/// <summary>
		/// StuId
		/// </summary>
		[DataMember]
		public long StuId { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// ClsName
		/// </summary>
		[DataMember]
		public string ClsName { get; set; }

		/// <summary>
		/// CreateTime
		/// </summary>
		[DataMember]
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// UpdateTime
		/// </summary>
		[DataMember]
		public DateTime UpdateTime { get; set; }

		#endregion 属性

		#region 列名/属性名

		public const string StuIdProperty = nameof(StuId);

		public const string NameProperty = nameof(Name);

		public const string ClsNameProperty = nameof(ClsName);

		public const string CreateTimeProperty = nameof(CreateTime);

		public const string UpdateTimeProperty = nameof(UpdateTime);

		#endregion 列名/属性名

		#region 辅助函数

		/// <summary>
		/// 获取所有列名/属性名，以字符串数组形式返回
		/// </summary>
		public static string[] Columns => new[]
		{
			StuIdProperty,
			NameProperty,
			ClsNameProperty,
			CreateTimeProperty,
			UpdateTimeProperty,
		};

		/// <summary>
		/// 获取数据库中此表的名字
		/// </summary>
		public static string GetDbTableName()
		{
			return "Student";
		}

		/// <summary>
		/// Convert转换函数，将数据表对应的DataTable转换成 List‹Student>
		/// </summary>
		public static List<Student> Convert(DataTable dt)
		{
			var result = new List<Student>();
			if(dt.Rows.Count < 1)
				return result;
			foreach(DataRow row in dt.Rows)
			{
				var tmpEntity = new Student();
				tmpEntity.StuId = (long)row[StuIdProperty];
				tmpEntity.Name = row[NameProperty].ToString();
				if(row[ClsNameProperty] == DBNull.Value)
					tmpEntity.ClsName = null;
				else
					tmpEntity.ClsName = row[ClsNameProperty].ToString();
				tmpEntity.CreateTime = (DateTime)row[CreateTimeProperty];
				tmpEntity.UpdateTime = (DateTime)row[UpdateTimeProperty];
				result.Add(tmpEntity);
			}
			return result;
		}

		#endregion 辅助函数

		#region 扩展属性
		#endregion 扩展属性
	}
}