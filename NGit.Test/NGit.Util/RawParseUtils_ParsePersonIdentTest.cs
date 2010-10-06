using System;
using NGit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_ParsePersonIdentTest : TestCase
	{
		public virtual void TestParsePersonIdent_legalCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("Me <me@example.com> 1234567890 -0700", new PersonIdent("Me", "me@example.com"
				, when, tz));
			AssertPersonIdent(" Me <me@example.com> 1234567890 -0700", new PersonIdent(" Me", 
				"me@example.com", when, tz));
			AssertPersonIdent("A U Thor <author@example.com> 1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor<author@example.com> 1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor<author@example.com>1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent(" A U Thor   < author@example.com > 1234567890 -0700", new PersonIdent
				(" A U Thor  ", " author@example.com ", when, tz));
			AssertPersonIdent("A U Thor<author@example.com>1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
		}

		public virtual void TestParsePersonIdent_fuzzyCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("A U Thor <author@example.com>,  C O. Miter <comiter@example.com> 1234567890 -0700"
				, new PersonIdent("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor <author@example.com> and others 1234567890 -0700", new 
				PersonIdent("A U Thor", "author@example.com", when, tz));
		}

		public virtual void TestParsePersonIdent_incompleteCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("Me <> 1234567890 -0700", new PersonIdent("Me", string.Empty, when
				, tz));
			AssertPersonIdent(" <me@example.com> 1234567890 -0700", new PersonIdent(string.Empty
				, "me@example.com", when, tz));
			AssertPersonIdent(" <> 1234567890 -0700", new PersonIdent(string.Empty, string.Empty
				, when, tz));
			AssertPersonIdent("<>", new PersonIdent(string.Empty, string.Empty, 0, 0));
			AssertPersonIdent(" <>", new PersonIdent(string.Empty, string.Empty, 0, 0));
			AssertPersonIdent("<me@example.com>", new PersonIdent(string.Empty, "me@example.com"
				, 0, 0));
			AssertPersonIdent(" <me@example.com>", new PersonIdent(string.Empty, "me@example.com"
				, 0, 0));
			AssertPersonIdent("Me <>", new PersonIdent("Me", string.Empty, 0, 0));
			AssertPersonIdent("Me <me@example.com>", new PersonIdent("Me", "me@example.com", 
				0, 0));
			AssertPersonIdent("Me <me@example.com> 1234567890", new PersonIdent("Me", "me@example.com"
				, 0, 0));
			AssertPersonIdent("Me <me@example.com> 1234567890 ", new PersonIdent("Me", "me@example.com"
				, 0, 0));
		}

		public virtual void TestParsePersonIdent_malformedCases()
		{
			AssertPersonIdent("Me me@example.com> 1234567890 -0700", null);
			AssertPersonIdent("Me <me@example.com 1234567890 -0700", null);
		}

		private void AssertPersonIdent(string line, PersonIdent expected)
		{
			PersonIdent actual = RawParseUtils.ParsePersonIdent(line);
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}
	}
}
