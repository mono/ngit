using System;
using NUnit.Framework;
using Sharpen;
using System.Threading;

namespace Sharpen.Test
{
	[TestFixture()]
	public class PipedStreams
	{
		[Test()]
		public void TestBigWrite ()
		{
			PipedInputStream pin = new PipedInputStream ();
			PipedOutputStream pout = new PipedOutputStream (pin);
			
			Random r = new Random (0);
			byte[] data = new byte [PipedInputStream.PIPE_SIZE * 3];
			r.NextBytes (data);
			
			ThreadPool.QueueUserWorkItem (delegate {
				pout.Write (data);
				pout.Close ();
			});
			int n = 0;
			byte[] buffer = new byte [100];
			int nr;
			while ((nr = pin.Read (buffer)) != -1) {
				for (int i=0; i < nr; i++)
					Assert.AreEqual (buffer[i], data[n++], "Position " + n);
			}
		}
	}
}

