using System;
using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class T0001_PersonIdent : TestCase
	{
		public virtual void Test001_NewIdent()
		{
			PersonIdent p = new PersonIdent("A U Thor", "author@example.com", Sharpen.Extensions.CreateDate
				(1142878501000L), Sharpen.Extensions.GetTimeZone("EST"));
			NUnit.Framework.Assert.AreEqual("A U Thor", p.GetName());
			NUnit.Framework.Assert.AreEqual("author@example.com", p.GetEmailAddress());
			NUnit.Framework.Assert.AreEqual(1142878501000L, p.GetWhen().GetTime());
			NUnit.Framework.Assert.AreEqual("A U Thor <author@example.com> 1142878501 -0500", 
				p.ToExternalString());
		}

		public virtual void Test002_NewIdent()
		{
			PersonIdent p = new PersonIdent("A U Thor", "author@example.com", Sharpen.Extensions.CreateDate
				(1142878501000L), Sharpen.Extensions.GetTimeZone("GMT+0230"));
			NUnit.Framework.Assert.AreEqual("A U Thor", p.GetName());
			NUnit.Framework.Assert.AreEqual("author@example.com", p.GetEmailAddress());
			NUnit.Framework.Assert.AreEqual(1142878501000L, p.GetWhen().GetTime());
			NUnit.Framework.Assert.AreEqual("A U Thor <author@example.com> 1142878501 +0230", 
				p.ToExternalString());
		}
	}
}
