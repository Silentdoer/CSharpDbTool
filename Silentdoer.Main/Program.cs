using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Silentdoer.Common;
using Silentdoer.Entities;

namespace Silentdoer.Main
{
	class Program
	{
		static void Main(string[] args)
		{
            var result = DalManager.GetAllRecordsOfTablePart(Student.GetDbTableName, new[] { Student.StuIdProperty, Student.NameProperty },
                dt =>
                {  // TODO converter，将DataTable转换为List<StudentPart>对象，这里可以改进。
                    var rst = new List<StudentPart>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var tmp = new StudentPart();
                        tmp.StuId = Convert.ToInt64(row[Student.StuIdProperty]);
                        tmp.Name = row[Student.NameProperty].ToString();
                        rst.Add(tmp);
                    }
                    return rst;
                });
            Console.WriteLine(result.Count);
            Console.WriteLine(result[0].Name);
            Console.WriteLine(result[result.Count - 1].Name);
			Console.ReadKey();
		}

		private class StudentPart
		{
			public long StuId;

			public string Name;
		}
	}
}
