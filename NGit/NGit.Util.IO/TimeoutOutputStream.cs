using System;
using System.Threading;
using NGit;
using NGit.Util.IO;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>OutputStream with a configurable timeout.</summary>
	/// <remarks>OutputStream with a configurable timeout.</remarks>
	public class TimeoutOutputStream : OutputStream
	{
		private readonly OutputStream dst;

		private readonly InterruptTimer myTimer;

		private int timeout;

		/// <summary>Wrap an output stream with a timeout on all write operations.</summary>
		/// <remarks>Wrap an output stream with a timeout on all write operations.</remarks>
		/// <param name="destination">
		/// base input stream (to write to). The stream must be
		/// interruptible (most socket streams are).
		/// </param>
		/// <param name="timer">timer to manage the timeouts during writes.</param>
		public TimeoutOutputStream(OutputStream destination, InterruptTimer timer)
		{
			dst = destination;
			myTimer = timer;
		}

		/// <returns>number of milliseconds before aborting a write.</returns>
		public virtual int GetTimeout()
		{
			return timeout;
		}

		/// <param name="millis">number of milliseconds before aborting a write. Must be &gt; 0.
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
		public override void Write(int b)
		{
			try
			{
				BeginWrite();
				dst.Write(b);
			}
			catch (ThreadInterruptedException)
			{
				throw WriteTimedOut();
			}
			finally
			{
				EndWrite();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(byte[] buf)
		{
			Write(buf, 0, buf.Length);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Write(byte[] buf, int off, int len)
		{
			try
			{
				BeginWrite();
				dst.Write(buf, off, len);
			}
			catch (ThreadInterruptedException)
			{
				throw WriteTimedOut();
			}
			finally
			{
				EndWrite();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Flush()
		{
			try
			{
				BeginWrite();
				dst.Flush();
			}
			catch (ThreadInterruptedException)
			{
				throw WriteTimedOut();
			}
			finally
			{
				EndWrite();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			try
			{
				BeginWrite();
				dst.Close();
			}
			catch (ThreadInterruptedException)
			{
				throw WriteTimedOut();
			}
			finally
			{
				EndWrite();
			}
		}

		private void BeginWrite()
		{
			myTimer.Begin(timeout);
		}

		private void EndWrite()
		{
			myTimer.End();
		}

		private static ThreadInterruptedException WriteTimedOut()
		{
			return new ThreadInterruptedException(JGitText.Get().writeTimedOut);
		}
	}
}
