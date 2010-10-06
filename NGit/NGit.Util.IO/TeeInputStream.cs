using System;
using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>Input stream that copies data read to another output stream.</summary>
	/// <remarks>
	/// Input stream that copies data read to another output stream.
	/// This stream is primarily useful with a
	/// <see cref="NGit.Util.TemporaryBuffer">NGit.Util.TemporaryBuffer</see>
	/// , where any
	/// data read or skipped by the caller is also duplicated into the temporary
	/// buffer. Later the temporary buffer can then be used instead of the original
	/// source stream.
	/// During close this stream copies any remaining data from the source stream
	/// into the destination stream.
	/// </remarks>
	public class TeeInputStream : InputStream
	{
		private byte[] skipBuffer;

		private InputStream src;

		private OutputStream dst;

		/// <summary>Initialize a tee input stream.</summary>
		/// <remarks>Initialize a tee input stream.</remarks>
		/// <param name="src">source stream to consume.</param>
		/// <param name="dst">
		/// destination to copy the source to as it is consumed. Typically
		/// this is a
		/// <see cref="NGit.Util.TemporaryBuffer">NGit.Util.TemporaryBuffer</see>
		/// .
		/// </param>
		public TeeInputStream(InputStream src, OutputStream dst)
		{
			this.src = src;
			this.dst = dst;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			byte[] b = SkipBuffer();
			int n = Read(b, 0, 1);
			return n == 1 ? b[0] & unchecked((int)(0xff)) : -1;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override long Skip(long cnt)
		{
			long skipped = 0;
			byte[] b = SkipBuffer();
			while (0 < cnt)
			{
				int n = src.Read(b, 0, (int)Math.Min(b.Length, cnt));
				if (n <= 0)
				{
					break;
				}
				dst.Write(b, 0, n);
				skipped += n;
				cnt -= n;
			}
			return skipped;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b, int off, int len)
		{
			if (len == 0)
			{
				return 0;
			}
			int n = src.Read(b, off, len);
			if (0 < n)
			{
				dst.Write(b, off, len);
			}
			return n;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			byte[] b = SkipBuffer();
			for (; ; )
			{
				int n = src.Read(b);
				if (n <= 0)
				{
					break;
				}
				dst.Write(b, 0, n);
			}
			dst.Close();
			src.Close();
		}

		private byte[] SkipBuffer()
		{
			if (skipBuffer == null)
			{
				skipBuffer = new byte[2048];
			}
			return skipBuffer;
		}
	}
}
