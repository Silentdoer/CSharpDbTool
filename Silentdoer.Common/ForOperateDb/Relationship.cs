using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Silentdoer.Common
{
	[DataContract]
	[Serializable]
	// 用于两个FilterInfo之间的并存关系，以前者为准，比如两个FilterInfo，第一个的是Or，第二个的是And；则它们之间的关系是Or
	public enum Relationship
	{
		[EnumMember]
		And,
		[EnumMember]
		Or
	}

	public static class RelationshipHelper
	{
		public static Relationship GetRelationshipEnum(string relationship)
		{
			var tmp = relationship.ToLower();
			return tmp.Equals("and") || tmp.Equals("&&") ? Relationship.And : Relationship.Or;
		}

		public static string GetRelationshipString(Relationship relationship)
		{
			return relationship == Relationship.And ? "&&" : "||";
		}
	}
}
