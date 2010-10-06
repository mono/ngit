using System;
using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// A
	/// <see cref="ByteWindow">ByteWindow</see>
	/// with an underlying byte array for storage.
	/// </summary>
	internal sealed class ByteArrayWindow : ByteWindow
	{
		private readonly byte[] array;

		internal ByteArrayWindow(PackFile pack, long o, byte[] b) : base(pack, o, b.Length
			)
		{
			array = b;
		}

		protected internal override int Copy(int p, byte[] b, int o, int n)
		{
			n = Math.Min(array.Length - p, n);
			System.Array.Copy(array, p, b, o, n);
			return n;
		}

		/// <exception cref="Sharpen.DataFormatException"></exception>
		protected internal override int SetInput(int pos, Inflater inf)
		{
			int n = array.Length - pos;
			inf.SetInput(array, pos, n);
			return n;
		}

		internal void Crc32(CRC32 @out, long pos, int cnt)
		{
			@out.Update(array, (int)(pos - start), cnt);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal void Write(OutputStream @out, long pos, int cnt)
		{
			@out.Write(array, (int)(pos - start), cnt);
		}

		/// <exception cref="Sharpen.DataFormatException"></exception>
		internal void Check(Inflater inf, byte[] tmp, long pos, int cnt)
		{
			inf.SetInput(array, (int)(pos - start), cnt);
			while (inf.Inflate(tmp, 0, tmp.Length) > 0)
			{
				continue;
			}
		}
	}
}
