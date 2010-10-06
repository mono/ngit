using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	internal class XInputStream : BufferedInputStream
	{
		private readonly byte[] intbuf = new byte[8];

		protected XInputStream(InputStream s) : base(s)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual byte[] ReadFully(int len)
		{
			lock (this)
			{
				byte[] b = new byte[len];
				ReadFully(b, 0, len);
				return b;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void ReadFully(byte[] b, int o, int len)
		{
			lock (this)
			{
				int r;
				while (len > 0 && (r = Read(b, o, len)) > 0)
				{
					o += r;
					len -= r;
				}
				if (len > 0)
				{
					throw new EOFException();
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual int ReadUInt8()
		{
			int r = Read();
			if (r < 0)
			{
				throw new EOFException();
			}
			return r;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual long ReadUInt32()
		{
			ReadFully(intbuf, 0, 4);
			return NB.DecodeUInt32(intbuf, 0);
		}
	}
}
