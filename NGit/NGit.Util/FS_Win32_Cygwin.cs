using System;
using System.Diagnostics;
using System.IO;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	internal class FS_Win32_Cygwin : FS_Win32
	{
		private static string cygpath;

		internal static bool Detect()
		{
			string path = AccessController.DoPrivileged(new _PrivilegedAction_58());
			if (path == null)
			{
				return false;
			}
			foreach (string p in path.Split(";"))
			{
				FilePath e = new FilePath(p, "cygpath.exe");
				if (e.IsFile())
				{
					cygpath = e.GetAbsolutePath();
					return true;
				}
			}
			return false;
		}

		private sealed class _PrivilegedAction_58 : PrivilegedAction<string>
		{
			public _PrivilegedAction_58()
			{
			}

			public string Run()
			{
				return Runtime.GetProperty("java.library.path");
			}
		}

		public override FilePath Resolve(FilePath dir, string pn)
		{
			try
			{
				Process p;
				p = Runtime.GetRuntime().Exec(new string[] { cygpath, "--windows", "--absolute", 
					pn }, null, dir);
				p.GetOutputStream().Close();
				BufferedReader lineRead = new BufferedReader(new InputStreamReader(p.GetInputStream
					(), "UTF-8"));
				string r = null;
				try
				{
					r = lineRead.ReadLine();
				}
				finally
				{
					lineRead.Close();
				}
				for (; ; )
				{
					try
					{
						if (p.WaitFor() == 0 && r != null && r.Length > 0)
						{
							return new FilePath(r);
						}
						break;
					}
					catch (Exception)
					{
					}
				}
			}
			catch (IOException)
			{
			}
			// Stop bothering me, I have a zombie to reap.
			// Fall through and use the default return.
			//
			return base.Resolve(dir, pn);
		}

		protected internal override FilePath UserHomeImpl()
		{
			string home = AccessController.DoPrivileged(new _PrivilegedAction_112());
			if (home == null || home.Length == 0)
			{
				return base.UserHomeImpl();
			}
			return Resolve(new FilePath("."), home);
		}

		private sealed class _PrivilegedAction_112 : PrivilegedAction<string>
		{
			public _PrivilegedAction_112()
			{
			}

			public string Run()
			{
				return Runtime.Getenv("HOME");
			}
		}
	}
}
