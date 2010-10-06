using System;
using System.Net;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Interface to read values from the system.</summary>
	/// <remarks>
	/// Interface to read values from the system.
	/// <p>
	/// When writing unit tests, extending this interface with a custom class
	/// permits to simulate an access to a system variable or property and
	/// permits to control the user's global configuration.
	/// </p>
	/// </remarks>
	public abstract class SystemReader
	{
		private sealed class _SystemReader_64 : SystemReader
		{
			public _SystemReader_64()
			{
			}

			private string hostname;

			public override string Getenv(string variable)
			{
				return Runtime.Getenv(variable);
			}

			public override string GetProperty(string key)
			{
				return Runtime.GetProperty(key);
			}

			public override FileBasedConfig OpenUserConfig(FS fs)
			{
				FilePath home = fs.UserHome();
				return new FileBasedConfig(new FilePath(home, ".gitconfig"), fs);
			}

			public override string GetHostname()
			{
				if (this.hostname == null)
				{
					try
					{
						IPAddress localMachine = Sharpen.Runtime.GetLocalHost();
						this.hostname = localMachine.ToString();
					}
					catch (UnknownHostException)
					{
						// we do nothing
						this.hostname = "localhost";
					}
				}
				return this.hostname;
			}

			public override long GetCurrentTime()
			{
				return Runtime.CurrentTimeMillis();
			}

			public override int GetTimezone(long when)
			{
				return System.TimeZoneInfo.Local.GetOffset(when) / (60 * 1000);
			}
		}

		private static SystemReader INSTANCE = new _SystemReader_64();

		/// <returns>the live instance to read system properties.</returns>
		public static SystemReader GetInstance()
		{
			return INSTANCE;
		}

		/// <param name="newReader">the new instance to use when accessing properties.</param>
		public static void SetInstance(SystemReader newReader)
		{
			INSTANCE = newReader;
		}

		/// <summary>Gets the hostname of the local host.</summary>
		/// <remarks>
		/// Gets the hostname of the local host. If no hostname can be found, the
		/// hostname is set to the default value "localhost".
		/// </remarks>
		/// <returns>the canonical hostname</returns>
		public abstract string GetHostname();

		/// <param name="variable">system variable to read</param>
		/// <returns>value of the system variable</returns>
		public abstract string Getenv(string variable);

		/// <param name="key">of the system property to read</param>
		/// <returns>value of the system property</returns>
		public abstract string GetProperty(string key);

		/// <param name="fs">
		/// the file system abstraction which will be necessary to
		/// perform certain file system operations.
		/// </param>
		/// <returns>the git configuration found in the user home</returns>
		public abstract FileBasedConfig OpenUserConfig(FS fs);

		/// <returns>the current system time</returns>
		public abstract long GetCurrentTime();

		/// <param name="when">TODO</param>
		/// <returns>the local time zone</returns>
		public abstract int GetTimezone(long when);
	}
}
