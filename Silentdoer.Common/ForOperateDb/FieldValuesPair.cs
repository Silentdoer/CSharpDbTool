using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silentdoer.Common
{
	public class FieldValuesPair
	{
		public FieldValuesPair() { }

		public FieldValuesPair(string field, object[] values)
		{
			Field = field;
			Values = values;
		}
		public string Field { get; set; }

		public object[] Values { get; set; }
	}
}