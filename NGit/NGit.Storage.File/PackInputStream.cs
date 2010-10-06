using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	internal class PackInputStream : InputStream
	{
		private readonly WindowCursor wc;

		private readonly PackFile pack;

		private long pos;

		/// <exception cref="System.IO.IOException"></exception>
		internal PackInputStream(PackFile pack, long pos, WindowCursor wc)
		{
			this.pack = pack;
			this.pos = pos;
			this.wc = wc;
			// Pin the first window, to ensure the pack is open and valid.
			//
			wc.Pin(pack, pos);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read(byte[] b, int off, int len)
		{
			int n = wc.Copy(pack, pos, b, off, len);
			pos += n;
			return n;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override int Read()
		{
			byte[] buf = new byte[1];
			int n = Read(buf, 0, 1);
			return n == 1 ? buf[0] & unchecked((int)(0xff)) : -1;
		}

		public override void Close()
		{
			wc.Release();
		}
	}
}
