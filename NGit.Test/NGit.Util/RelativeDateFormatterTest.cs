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
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class RelativeDateFormatterTest
	{
		private void AssertFormat(long ageFromNow, long timeUnit, string expectedFormat)
		{
			DateTime d = Sharpen.Extensions.CreateDate(Runtime.CurrentTimeMillis() - ageFromNow
				 * timeUnit);
			string s = RelativeDateFormatter.Format(d);
			NUnit.Framework.Assert.AreEqual(expectedFormat, s);
		}

		[NUnit.Framework.Test]
		public virtual void TestFuture()
		{
			AssertFormat(-100, RelativeDateFormatter.YEAR_IN_MILLIS, "in the future");
			AssertFormat(-1, RelativeDateFormatter.SECOND_IN_MILLIS, "in the future");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatSeconds()
		{
			AssertFormat(1, RelativeDateFormatter.SECOND_IN_MILLIS, "1 seconds ago");
			AssertFormat(89, RelativeDateFormatter.SECOND_IN_MILLIS, "89 seconds ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatMinutes()
		{
			AssertFormat(90, RelativeDateFormatter.SECOND_IN_MILLIS, "2 minutes ago");
			AssertFormat(3, RelativeDateFormatter.MINUTE_IN_MILLIS, "3 minutes ago");
			AssertFormat(60, RelativeDateFormatter.MINUTE_IN_MILLIS, "60 minutes ago");
			AssertFormat(89, RelativeDateFormatter.MINUTE_IN_MILLIS, "89 minutes ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatHours()
		{
			AssertFormat(90, RelativeDateFormatter.MINUTE_IN_MILLIS, "2 hours ago");
			AssertFormat(149, RelativeDateFormatter.MINUTE_IN_MILLIS, "2 hours ago");
			AssertFormat(35, RelativeDateFormatter.HOUR_IN_MILLIS, "35 hours ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatDays()
		{
			AssertFormat(36, RelativeDateFormatter.HOUR_IN_MILLIS, "2 days ago");
			AssertFormat(13, RelativeDateFormatter.DAY_IN_MILLIS, "13 days ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatWeeks()
		{
			AssertFormat(14, RelativeDateFormatter.DAY_IN_MILLIS, "2 weeks ago");
			AssertFormat(69, RelativeDateFormatter.DAY_IN_MILLIS, "10 weeks ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatMonths()
		{
			AssertFormat(70, RelativeDateFormatter.DAY_IN_MILLIS, "2 months ago");
			AssertFormat(75, RelativeDateFormatter.DAY_IN_MILLIS, "3 months ago");
			AssertFormat(364, RelativeDateFormatter.DAY_IN_MILLIS, "12 months ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatYearsMonths()
		{
			AssertFormat(366, RelativeDateFormatter.DAY_IN_MILLIS, "1 year, 0 month ago");
			AssertFormat(380, RelativeDateFormatter.DAY_IN_MILLIS, "1 year, 1 month ago");
			AssertFormat(410, RelativeDateFormatter.DAY_IN_MILLIS, "1 year, 2 months ago");
			AssertFormat(2, RelativeDateFormatter.YEAR_IN_MILLIS, "2 years, 0 month ago");
			AssertFormat(1824, RelativeDateFormatter.DAY_IN_MILLIS, "4 years, 12 months ago");
		}

		[NUnit.Framework.Test]
		public virtual void TestFormatYears()
		{
			AssertFormat(5, RelativeDateFormatter.YEAR_IN_MILLIS, "5 years ago");
			AssertFormat(60, RelativeDateFormatter.YEAR_IN_MILLIS, "60 years ago");
		}
	}
}
