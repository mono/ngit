using System.IO;
using System.Threading;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>Thread to copy from an input stream to an output stream.</summary>
	/// <remarks>Thread to copy from an input stream to an output stream.</remarks>
	internal class StreamCopyThread : Sharpen.Thread
	{
		private const int BUFFER_SIZE = 1024;

		private readonly InputStream src;

		private readonly OutputStream dst;

		private volatile bool done;

		/// <summary>Create a thread to copy data from an input stream to an output stream.</summary>
		/// <remarks>Create a thread to copy data from an input stream to an output stream.</remarks>
		/// <param name="i">
		/// stream to copy from. The thread terminates when this stream
		/// reaches EOF. The thread closes this stream before it exits.
		/// </param>
		/// <param name="o">
		/// stream to copy into. The destination stream is automatically
		/// closed when the thread terminates.
		/// </param>
		public StreamCopyThread(InputStream i, OutputStream o)
		{
			SetName(Sharpen.Thread.CurrentThread().GetName() + "-StreamCopy");
			src = i;
			dst = o;
		}

		/// <summary>Request the thread to flush the output stream as soon as possible.</summary>
		/// <remarks>
		/// Request the thread to flush the output stream as soon as possible.
		/// <p>
		/// This is an asynchronous request to the thread. The actual flush will
		/// happen at some future point in time, when the thread wakes up to process
		/// the request.
		/// </remarks>
		public virtual void Flush()
		{
			Interrupt();
		}

		/// <summary>Request that the thread terminate, and wait for it.</summary>
		/// <remarks>
		/// Request that the thread terminate, and wait for it.
		/// <p>
		/// This method signals to the copy thread that it should stop as soon as
		/// there is no more IO occurring.
		/// </remarks>
		/// <exception cref="System.Exception">the calling thread was interrupted.</exception>
		public virtual void Halt()
		{
			for (; ; )
			{
				Join(250);
				if (IsAlive())
				{
					done = true;
					Interrupt();
				}
				else
				{
					break;
				}
			}
		}

		public override void Run()
		{
			try
			{
				byte[] buf = new byte[BUFFER_SIZE];
				int interruptCounter = 0;
				for (; ; )
				{
					try
					{
						if (interruptCounter > 0)
						{
							dst.Flush();
							interruptCounter--;
						}
						if (done)
						{
							break;
						}
						int n;
						try
						{
							n = src.Read(buf);
						}
						catch (ThreadInterruptedException)
						{
							interruptCounter++;
							continue;
						}
						if (n < 0)
						{
							break;
						}
						bool writeInterrupted = false;
						for (; ; )
						{
							try
							{
								dst.Write(buf, 0, n);
							}
							catch (ThreadInterruptedException)
							{
								writeInterrupted = true;
								continue;
							}
							// set interrupt status, which will be checked
							// when we block in src.read
							if (writeInterrupted)
							{
								Interrupt();
							}
							break;
						}
					}
					catch (IOException)
					{
						break;
					}
				}
			}
			finally
			{
				try
				{
					src.Close();
				}
				catch (IOException)
				{
				}
				// Ignore IO errors on close
				try
				{
					dst.Close();
				}
				catch (IOException)
				{
				}
			}
		}
		// Ignore IO errors on close
	}
}
