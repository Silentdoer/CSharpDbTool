using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silentdoer.Common
{
	public class FieldValuePair
	{
		public FieldValuePair() { }

		public FieldValuePair(string field, object value)
		{
			Field = field;
			Value = value;
		}

		public string Field { get; set; }

		public object Value { get; set; }
	}
}