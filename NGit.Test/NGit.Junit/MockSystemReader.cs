using System;
using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Junit
{
	public class MockSystemReader : SystemReader
	{
		internal readonly IDictionary<string, string> values = new Dictionary<string, string
			>();

		internal FileBasedConfig userGitConfig;

		public MockSystemReader()
		{
			Init(Constants.OS_USER_NAME_KEY);
			Init(Constants.GIT_AUTHOR_NAME_KEY);
			Init(Constants.GIT_AUTHOR_EMAIL_KEY);
			Init(Constants.GIT_COMMITTER_NAME_KEY);
			Init(Constants.GIT_COMMITTER_EMAIL_KEY);
			userGitConfig = new _FileBasedConfig_70(null, null);
		}

		private sealed class _FileBasedConfig_70 : FileBasedConfig
		{
			public _FileBasedConfig_70(FilePath baseArg1, FS baseArg2) : base(baseArg1, baseArg2
				)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
			public override void Load()
			{
			}

			// Do nothing
			public override bool IsOutdated()
			{
				return false;
			}
		}

		private void Init(string n)
		{
			SetProperty(n, n);
		}

		public virtual void ClearProperties()
		{
			values.Clear();
		}

		public virtual void SetProperty(string key, string value)
		{
			values.Put(key, value);
		}

		public override string Getenv(string variable)
		{
			return values.Get(variable);
		}

		public override string GetProperty(string key)
		{
			return values.Get(key);
		}

		public override FileBasedConfig OpenUserConfig(FS fs)
		{
			return userGitConfig;
		}

		public override string GetHostname()
		{
			return "fake.host.example.com";
		}

		public override long GetCurrentTime()
		{
			return 1250379778668L;
		}

		// Sat Aug 15 20:12:58 GMT-03:30 2009
		public override int GetTimezone(long when)
		{
			return Sharpen.Extensions.GetTimeZone("GMT-03:30").GetOffset(when) / (60 * 1000);
		}
	}
}
