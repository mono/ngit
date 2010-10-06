using Sharpen;

namespace NGit.Util.IO
{
	/// <summary>An input stream which canonicalizes EOLs bytes on the fly to '\n'.</summary>
	/// <remarks>
	/// An input stream which canonicalizes EOLs bytes on the fly to '\n'.
	/// Note: Make sure to apply this InputStream only to text files!
	/// </remarks>
	public class EolCanonicalizingInputStream : InputStream
	{
		private readonly byte[] single = new byte[1];

		private readonly byte[] buf = new byte[8096];

		private readonly InputStream @in;

		private int cnt;

		private int ptr;

		/// <summary>Creates a new InputStream, wrapping the specified stream</summary>
		/// <param name="in">raw input stream</param>
		public EolCanonicalizingInputStream(InputStream @in)
		{
			this.@in = @in;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			int read = Read(single, 0, 1);
			return read == 1 ? single[0] & unchecked((int)(0xff)) : -1;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] bs, int off, int len)
		{
			if (len == 0)
			{
				return 0;
			}
			if (cnt == -1)
			{
				return -1;
			}
			int startOff = off;
			int end = off + len;
			while (off < end)
			{
				if (ptr == cnt && !FillBuffer())
				{
					break;
				}
				byte b = buf[ptr++];
				if (b != '\r')
				{
					bs[off++] = b;
					continue;
				}
				if (ptr == cnt && !FillBuffer())
				{
					bs[off++] = (byte)('\r');
					break;
				}
				if (buf[ptr] == '\n')
				{
					bs[off++] = (byte)('\n');
					ptr++;
				}
				else
				{
					bs[off++] = (byte)('\r');
				}
			}
			return startOff == off ? -1 : off - startOff;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			@in.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private bool FillBuffer()
		{
			cnt = @in.Read(buf, 0, buf.Length);
			if (cnt < 1)
			{
				return false;
			}
			ptr = 0;
			return true;
		}
	}
}
