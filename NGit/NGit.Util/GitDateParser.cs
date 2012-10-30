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
using System.Collections.Generic;
using System.Text;
using NGit.Internal;
using NGit.Util;
using Sharpen;
using System.Threading;
using System.Globalization;

namespace NGit.Util
{
	/// <summary>
	/// Parses strings with time and date specifications into
	/// <see cref="System.DateTime">System.DateTime</see>
	/// .
	/// When git needs to parse strings specified by the user this parser can be
	/// used. One example is the parsing of the config parameter gc.pruneexpire. The
	/// parser can handle only subset of what native gits approxidate parser
	/// understands.
	/// </summary>
	public class GitDateParser
	{
		/// <summary>The Date representing never.</summary>
		/// <remarks>
		/// The Date representing never. Though this is a concrete value, most
		/// callers are adviced to avoid depending on the actual value.
		/// </remarks>
		public static readonly DateTime NEVER = Sharpen.Extensions.CreateDate(long.MaxValue
			);

		private sealed class _ThreadLocal_74 : ThreadLocal<IDictionary<GitDateParser.ParseableSimpleDateFormat
			, SimpleDateFormat>>
		{
			public _ThreadLocal_74()
				: base (() => new Dictionary<GitDateParser.ParseableSimpleDateFormat, SimpleDateFormat>())
			{
			}
		}

		private static ThreadLocal<IDictionary<GitDateParser.ParseableSimpleDateFormat, SimpleDateFormat
			>> formatCache = new _ThreadLocal_74();

		// Gets an instance of a SimpleDateFormat. If there is not already an
		// appropriate instance in the (ThreadLocal) cache the create one and put in
		// into the cache
		private static SimpleDateFormat GetDateFormat(GitDateParser.ParseableSimpleDateFormat
			 f)
		{
			IDictionary<GitDateParser.ParseableSimpleDateFormat, SimpleDateFormat> map = formatCache
				.Value;
			SimpleDateFormat dateFormat = map.Get(f);
			if (dateFormat != null)
			{
				return dateFormat;
			}
			SimpleDateFormat df = SystemReader.GetInstance().GetSimpleDateFormat(f.formatStr);
			map.Put(f, df);
			return df;
		}

		public class ParseableSimpleDateFormat
		{
			public static GitDateParser.ParseableSimpleDateFormat ISO = new GitDateParser.ParseableSimpleDateFormat
				("yyyy-MM-dd HH:mm:ss Z");

			public static GitDateParser.ParseableSimpleDateFormat RFC = new GitDateParser.ParseableSimpleDateFormat
				("EEE, dd MMM yyyy HH:mm:ss Z");

			public static GitDateParser.ParseableSimpleDateFormat SHORT = new GitDateParser.ParseableSimpleDateFormat
				("yyyy-MM-dd");

			public static GitDateParser.ParseableSimpleDateFormat SHORT_WITH_DOTS_REVERSE = new 
				GitDateParser.ParseableSimpleDateFormat("dd.MM.yyyy");

			public static GitDateParser.ParseableSimpleDateFormat SHORT_WITH_DOTS = new GitDateParser.ParseableSimpleDateFormat
				("yyyy.MM.dd");

			public static GitDateParser.ParseableSimpleDateFormat SHORT_WITH_SLASH = new GitDateParser.ParseableSimpleDateFormat
				("MM/dd/yyyy");

			public static GitDateParser.ParseableSimpleDateFormat DEFAULT = new GitDateParser.ParseableSimpleDateFormat
				("EEE MMM dd HH:mm:ss yyyy Z");

			public static GitDateParser.ParseableSimpleDateFormat LOCAL = new GitDateParser.ParseableSimpleDateFormat
				("EEE MMM dd HH:mm:ss yyyy");

			// An enum of all those formats which this parser can parse with the help of
			// a SimpleDateFormat. There are other formats (e.g. the relative formats
			// like "yesterday" or "1 week ago") which this parser can parse but which
			// are not listed here because they are parsed without the help of a
			// SimpleDateFormat.
			//
			//
			//
			//
			//
			//
			//
			public static GitDateParser.ParseableSimpleDateFormat[] Values()
			{
				return new GitDateParser.ParseableSimpleDateFormat[] { ISO, RFC, SHORT, SHORT_WITH_DOTS_REVERSE
					, SHORT_WITH_DOTS, SHORT_WITH_SLASH, DEFAULT, LOCAL };
			}

			public string formatStr;

			private ParseableSimpleDateFormat(string formatStr)
			{
				this.formatStr = formatStr;
			}
		}

		/// <summary>
		/// Parses a string into a
		/// <see cref="System.DateTime">System.DateTime</see>
		/// . Since this parser also supports
		/// relative formats (e.g. "yesterday") the caller can specify the reference
		/// date. These types of strings can be parsed:
		/// <ul>
		/// <li>"never"</li>
		/// <li>"now"</li>
		/// <li>"yesterday"</li>
		/// <li>"(x) years|months|weeks|days|hours|minutes|seconds ago"<br />
		/// Multiple specs can be combined like in "2 weeks 3 days ago". Instead of
		/// ' ' one can use '.' to seperate the words</li>
		/// <li>"yyyy-MM-dd HH:mm:ss Z" (ISO)</li>
		/// <li>"EEE, dd MMM yyyy HH:mm:ss Z" (RFC)</li>
		/// <li>"yyyy-MM-dd"</li>
		/// <li>"yyyy.MM.dd"</li>
		/// <li>"MM/dd/yyyy",</li>
		/// <li>"dd.MM.yyyy"</li>
		/// <li>"EEE MMM dd HH:mm:ss yyyy Z" (DEFAULT)</li>
		/// <li>"EEE MMM dd HH:mm:ss yyyy" (LOCAL)</li>
		/// </ul>
		/// </summary>
		/// <param name="dateStr">the string to be parsed</param>
		/// <param name="now">
		/// the base date which is used for the calculation of relative
		/// formats. E.g. if baseDate is "25.8.2012" then parsing of the
		/// string "1 week ago" would result in a date corresponding to
		/// "18.8.2012". This is used when a JGit command calls this
		/// parser often but wants a consistent starting point for calls.<br />
		/// If set to <code>null</code> then the current time will be used
		/// instead.
		/// </param>
		/// <returns>
		/// the parsed
		/// <see cref="System.DateTime">System.DateTime</see>
		/// </returns>
		/// <exception cref="Sharpen.ParseException">if the given dateStr was not recognized</exception>
		public static DateTime Parse(string dateStr, JavaCalendar now)
		{
			dateStr = dateStr.Trim();
			DateTime? ret;
			if (Sharpen.Runtime.EqualsIgnoreCase("never", dateStr))
			{
				return NEVER;
			}
			ret = Parse_relative(dateStr, now);
			if (ret != null)
			{
				return ret.Value;
			}
			foreach (GitDateParser.ParseableSimpleDateFormat f in GitDateParser.ParseableSimpleDateFormat
				.Values())
			{
				try
				{
					return Parse_simple(dateStr, f);
				}
				catch (ParseException)
				{
				}
			}
			// simply proceed with the next parser
			GitDateParser.ParseableSimpleDateFormat[] values = GitDateParser.ParseableSimpleDateFormat
				.Values();
			StringBuilder allFormats = new StringBuilder("\"").Append(values[0].formatStr);
			for (int i = 1; i < values.Length; i++)
			{
				allFormats.Append("\", \"").Append(values[i].formatStr);
			}
			allFormats.Append("\"");
			throw new ParseException(MessageFormat.Format(JGitText.Get().cannotParseDate, dateStr
				, allFormats.ToString()), 0);
		}

		// tries to parse a string with the formats supported by SimpleDateFormat
		/// <exception cref="Sharpen.ParseException"></exception>
		private static DateTime Parse_simple(string dateStr, GitDateParser.ParseableSimpleDateFormat
			 f)
		{
			SimpleDateFormat dateFormat = GetDateFormat(f);
			dateFormat.SetLenient(false);
			return dateFormat.Parse(dateStr);
		}

		// tries to parse a string with a relative time specification
		private static DateTime? Parse_relative(string dateStr, JavaCalendar now)
		{
			JavaCalendar cal;
			SystemReader sysRead = SystemReader.GetInstance();
			// check for the static words "yesterday" or "now"
			if ("now".Equals(dateStr))
			{
				return ((now == null) ? Sharpen.Extensions.CreateDate(sysRead.GetCurrentTime()) : 
					now.GetTime());
			}
			if (now == null)
			{
				cal = new JavaGregorianCalendar(sysRead.GetTimeZone(), sysRead.GetLocale());
				cal.SetTimeInMillis(sysRead.GetCurrentTime());
			}
			else
			{
				cal = (JavaCalendar)now.Clone();
			}
			if ("yesterday".Equals(dateStr))
			{
				cal.Add(JavaCalendar.DATE, -1);
				cal.Set(JavaCalendar.HOUR_OF_DAY, 0);
				cal.Set(JavaCalendar.MINUTE, 0);
				cal.Set(JavaCalendar.SECOND, 0);
				cal.Set(JavaCalendar.MILLISECOND, 0);
				cal.Set(JavaCalendar.MILLISECOND, 0);
				return cal.GetTime();
			}
			// parse constructs like "3 days ago", "5.week.2.day.ago"
			string[] parts = dateStr.Split("\\.| ");
			int partsLength = parts.Length;
			// check we have an odd number of parts (at least 3) and that the last
			// part is "ago"
			if (partsLength < 3 || (partsLength & 1) == 0 || !"ago".Equals(parts[parts.Length
				 - 1]))
			{
				return null;
			}
			int number;
			for (int i = 0; i < parts.Length - 2; i += 2)
			{
				try
				{
					number = System.Convert.ToInt32(parts[i]);
				}
				catch (FormatException)
				{
					return null;
				}
				if ("year".Equals(parts[i + 1]) || "years".Equals(parts[i + 1]))
				{
					cal.Add(JavaCalendar.YEAR, -number);
				}
				else
				{
					if ("month".Equals(parts[i + 1]) || "months".Equals(parts[i + 1]))
					{
						cal.Add(JavaCalendar.MONTH, -number);
					}
					else
					{
						if ("week".Equals(parts[i + 1]) || "weeks".Equals(parts[i + 1]))
						{
							cal.Add(JavaCalendar.WEEK_OF_YEAR, -number);
						}
						else
						{
							if ("day".Equals(parts[i + 1]) || "days".Equals(parts[i + 1]))
							{
								cal.Add(JavaCalendar.DATE, -number);
							}
							else
							{
								if ("hour".Equals(parts[i + 1]) || "hours".Equals(parts[i + 1]))
								{
									cal.Add(JavaCalendar.HOUR_OF_DAY, -number);
								}
								else
								{
									if ("minute".Equals(parts[i + 1]) || "minutes".Equals(parts[i + 1]))
									{
										cal.Add(JavaCalendar.MINUTE, -number);
									}
									else
									{
										if ("second".Equals(parts[i + 1]) || "seconds".Equals(parts[i + 1]))
										{
											cal.Add(JavaCalendar.SECOND, -number);
										}
										else
										{
											return null;
										}
									}
								}
							}
						}
					}
				}
			}
			return cal.GetTime();
		}
	}
}
