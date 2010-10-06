using System;
using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// A window for accessing git packs using a
	/// <see cref="Sharpen.ByteBuffer">Sharpen.ByteBuffer</see>
	/// for storage.
	/// </summary>
	/// <seealso cref="ByteWindow">ByteWindow</seealso>
	internal sealed class ByteBufferWindow : ByteWindow
	{
		private readonly ByteBuffer buffer;

		internal ByteBufferWindow(PackFile pack, long o, ByteBuffer b) : base(pack, o, b.
			Capacity())
		{
			buffer = b;
		}

		protected internal override int Copy(int p, byte[] b, int o, int n)
		{
			ByteBuffer s = buffer.Slice();
			s.Position(p);
			n = Math.Min(s.Remaining(), n);
			s.Get(b, o, n);
			return n;
		}

		/// <exception cref="Sharpen.DataFormatException"></exception>
		protected internal override int SetInput(int pos, Inflater inf)
		{
			ByteBuffer s = buffer.Slice();
			s.Position(pos);
			byte[] tmp = new byte[Math.Min(s.Remaining(), 512)];
			s.Get(tmp, 0, tmp.Length);
			inf.SetInput(tmp, 0, tmp.Length);
			return tmp.Length;
		}
	}
}
