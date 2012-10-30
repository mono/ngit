/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using NGit.Junit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class GitDateParserTest
	{
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			MockSystemReader mockSystemReader = new MockSystemReader();
			SystemReader.SetInstance(mockSystemReader);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Yesterday()
		{
			GregorianCalendar cal = new GregorianCalendar(SystemReader.GetInstance().GetTimeZone
				(), SystemReader.GetInstance().GetLocale());
			DateTime parse = GitDateParser.Parse("yesterday", cal);
			cal.Add(Calendar.DATE, -1);
			cal.Set(Calendar.HOUR_OF_DAY, 0);
			cal.Set(Calendar.MINUTE, 0);
			cal.Set(Calendar.SECOND, 0);
			cal.Set(Calendar.MILLISECOND, 0);
			cal.Set(Calendar.MILLISECOND, 0);
			NUnit.Framework.Assert.AreEqual(cal.GetTime(), parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Never()
		{
			GregorianCalendar cal = new GregorianCalendar(SystemReader.GetInstance().GetTimeZone
				(), SystemReader.GetInstance().GetLocale());
			DateTime parse = GitDateParser.Parse("never", cal);
			NUnit.Framework.Assert.AreEqual(GitDateParser.NEVER, parse);
			parse = GitDateParser.Parse("never", null);
			NUnit.Framework.Assert.AreEqual(GitDateParser.NEVER, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Now()
		{
			string dateStr = "2007-02-21 15:35:00 +0100";
			DateTime refDate = SystemReader.GetInstance().GetSimpleDateFormat("yyyy-MM-dd HH:mm:ss Z"
				).Parse(dateStr);
			GregorianCalendar cal = new GregorianCalendar(SystemReader.GetInstance().GetTimeZone
				(), SystemReader.GetInstance().GetLocale());
			cal.SetTime(refDate);
			DateTime parse = GitDateParser.Parse("now", cal);
			NUnit.Framework.Assert.AreEqual(refDate, parse);
			long t1 = SystemReader.GetInstance().GetCurrentTime();
			parse = GitDateParser.Parse("now", null);
			long t2 = SystemReader.GetInstance().GetCurrentTime();
			NUnit.Framework.Assert.IsTrue(t2 >= parse.GetTime() && parse.GetTime() >= t1);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void WeeksAgo()
		{
			string dateStr = "2007-02-21 15:35:00 +0100";
			SimpleDateFormat df = SystemReader.GetInstance().GetSimpleDateFormat("yyyy-MM-dd HH:mm:ss Z"
				);
			DateTime refDate = df.Parse(dateStr);
			GregorianCalendar cal = new GregorianCalendar(SystemReader.GetInstance().GetTimeZone
				(), SystemReader.GetInstance().GetLocale());
			cal.SetTime(refDate);
			DateTime parse = GitDateParser.Parse("2 weeks ago", cal);
			NUnit.Framework.Assert.AreEqual(df.Parse("2007-02-07 15:35:00 +0100"), parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void DaysAndWeeksAgo()
		{
			string dateStr = "2007-02-21 15:35:00 +0100";
			SimpleDateFormat df = SystemReader.GetInstance().GetSimpleDateFormat("yyyy-MM-dd HH:mm:ss Z"
				);
			DateTime refDate = df.Parse(dateStr);
			GregorianCalendar cal = new GregorianCalendar(SystemReader.GetInstance().GetTimeZone
				(), SystemReader.GetInstance().GetLocale());
			cal.SetTime(refDate);
			DateTime parse = GitDateParser.Parse("2 weeks ago", cal);
			NUnit.Framework.Assert.AreEqual(df.Parse("2007-02-07 15:35:00 +0100"), parse);
			parse = GitDateParser.Parse("3 days 2 weeks ago", cal);
			NUnit.Framework.Assert.AreEqual(df.Parse("2007-02-04 15:35:00 +0100"), parse);
			parse = GitDateParser.Parse("3.day.2.week.ago", cal);
			NUnit.Framework.Assert.AreEqual(df.Parse("2007-02-04 15:35:00 +0100"), parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Iso()
		{
			string dateStr = "2007-02-21 15:35:00 +0100";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("yyyy-MM-dd HH:mm:ss Z"
				).Parse(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Rfc()
		{
			string dateStr = "Wed, 21 Feb 2007 15:35:00 +0100";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("EEE, dd MMM yyyy HH:mm:ss Z"
				).Parse(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void ShortFmt()
		{
			string dateStr = "2007-02-21";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("yyyy-MM-dd").Parse
				(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void ShortWithDots()
		{
			string dateStr = "2007.02.21";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("yyyy.MM.dd").Parse
				(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void ShortWithSlash()
		{
			string dateStr = "02/21/2007";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("MM/dd/yyyy").Parse
				(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void ShortWithDotsReverse()
		{
			string dateStr = "21.02.2007";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("dd.MM.yyyy").Parse
				(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void DefaultFmt()
		{
			string dateStr = "Wed Feb 21 15:35:00 2007 +0100";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("EEE MMM dd HH:mm:ss yyyy Z"
				).Parse(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}

		/// <exception cref="Sharpen.ParseException"></exception>
		[NUnit.Framework.Test]
		public virtual void Local()
		{
			string dateStr = "Wed Feb 21 15:35:00 2007";
			DateTime exp = SystemReader.GetInstance().GetSimpleDateFormat("EEE MMM dd HH:mm:ss yyyy"
				).Parse(dateStr);
			DateTime parse = GitDateParser.Parse(dateStr, null);
			NUnit.Framework.Assert.AreEqual(exp, parse);
		}
	}
}
