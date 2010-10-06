using System;
using System.Reflection;
using System.Security;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	internal class FS_POSIX_Java6 : FS
	{
		private static readonly MethodInfo canExecute;

		private static readonly MethodInfo setExecute;

		static FS_POSIX_Java6()
		{
			canExecute = NeedMethod(typeof(FilePath), "canExecute");
			setExecute = NeedMethod(typeof(FilePath), "setExecutable", typeof(bool));
		}

		internal static bool Detect()
		{
			return canExecute != null && setExecute != null;
		}

		private static MethodInfo NeedMethod(Type on, string name, params Type[]
			 args)
		{
			try
			{
				return on.GetMethod(name, args);
			}
			catch (SecurityException)
			{
				return null;
			}
			catch (NoSuchMethodException)
			{
				return null;
			}
		}

		public override bool SupportsExecute()
		{
			return true;
		}

		public override bool CanExecute(FilePath f)
		{
			try
			{
				object r = canExecute.Invoke(f, (object[])null);
				return ((bool)r);
			}
			catch (ArgumentException e)
			{
				throw new Error(e);
			}
			catch (MemberAccessException e)
			{
				throw new Error(e);
			}
			catch (TargetInvocationException e)
			{
				throw new Error(e);
			}
		}

		public override bool SetExecute(FilePath f, bool canExec)
		{
			try
			{
				object r;
				r = setExecute.Invoke(f, new object[] { Sharpen.Extensions.ValueOf(canExec) });
				return ((bool)r);
			}
			catch (ArgumentException e)
			{
				throw new Error(e);
			}
			catch (MemberAccessException e)
			{
				throw new Error(e);
			}
			catch (TargetInvocationException e)
			{
				throw new Error(e);
			}
		}

		public override bool RetryFailedLockFileCommit()
		{
			return false;
		}
	}
}
