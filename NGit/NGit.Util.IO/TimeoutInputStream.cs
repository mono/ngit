using System;
using System.Threading;
using NGit;
using NGit.Util.IO;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>InputStream with a configurable timeout.</summary>
	/// <remarks>InputStream with a configurable timeout.</remarks>
	public class TimeoutInputStream : FilterInputStream
	{
		private readonly InterruptTimer myTimer;

		private int timeout;

		/// <summary>Wrap an input stream with a timeout on all read operations.</summary>
		/// <remarks>Wrap an input stream with a timeout on all read operations.</remarks>
		/// <param name="src">
		/// base input stream (to read from). The stream must be
		/// interruptible (most socket streams are).
		/// </param>
		/// <param name="timer">timer to manage the timeouts during reads.</param>
		public TimeoutInputStream(InputStream src, InterruptTimer timer) : base(src)
		{
			myTimer = timer;
		}

		/// <returns>number of milliseconds before aborting a read.</returns>
		public virtual int GetTimeout()
		{
			return timeout;
		}

		/// <param name="millis">number of milliseconds before aborting a read. Must be &gt; 0.
		/// 	</param>
		public virtual void SetTimeout(int millis)
		{
			if (millis < 0)
			{
				throw new ArgumentException(MessageFormat.Format(JGitText.Get().invalidTimeout, millis
					));
			}
			timeout = millis;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			try
			{
				BeginRead();
				return base.Read();
			}
			catch (ThreadInterruptedException)
			{
				throw ReadTimedOut();
			}
			finally
			{
				EndRead();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] buf)
		{
			return Read(buf, 0, buf.Length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] buf, int off, int cnt)
		{
			try
			{
				BeginRead();
				return base.Read(buf, off, cnt);
			}
			catch (ThreadInterruptedException)
			{
				throw ReadTimedOut();
			}
			finally
			{
				EndRead();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override long Skip(long cnt)
		{
			try
			{
				BeginRead();
				return base.Skip(cnt);
			}
			catch (ThreadInterruptedException)
			{
				throw ReadTimedOut();
			}
			finally
			{
				EndRead();
			}
		}

		private void BeginRead()
		{
			myTimer.Begin(timeout);
		}

		private void EndRead()
		{
			myTimer.End();
		}

		private static ThreadInterruptedException ReadTimedOut()
		{
			return new ThreadInterruptedException(JGitText.Get().readTimedOut);
		}
	}
}
