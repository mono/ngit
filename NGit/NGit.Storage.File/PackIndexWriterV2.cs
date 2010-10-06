using NGit.Storage.File;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Creates the version 2 pack table of contents files.</summary>
	/// <remarks>Creates the version 2 pack table of contents files.</remarks>
	/// <seealso cref="PackIndexWriter">PackIndexWriter</seealso>
	/// <seealso cref="PackIndexV2">PackIndexV2</seealso>
	internal class PackIndexWriterV2 : PackIndexWriter
	{
		protected internal PackIndexWriterV2(OutputStream dst) : base(dst)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override void WriteImpl()
		{
			WriteTOC(2);
			WriteFanOutTable();
			WriteObjectNames();
			WriteCRCs();
			WriteOffset32();
			WriteOffset64();
			WriteChecksumFooter();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteObjectNames()
		{
			foreach (PackedObjectInfo oe in entries)
			{
				oe.CopyRawTo(@out);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteCRCs()
		{
			foreach (PackedObjectInfo oe in entries)
			{
				NB.EncodeInt32(tmp, 0, oe.GetCRC());
				@out.Write(tmp, 0, 4);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteOffset32()
		{
			int o64 = 0;
			foreach (PackedObjectInfo oe in entries)
			{
				long o = oe.GetOffset();
				if (o < int.MaxValue)
				{
					NB.EncodeInt32(tmp, 0, (int)o);
				}
				else
				{
					NB.EncodeInt32(tmp, 0, (1 << 31) | o64++);
				}
				@out.Write(tmp, 0, 4);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteOffset64()
		{
			foreach (PackedObjectInfo oe in entries)
			{
				long o = oe.GetOffset();
				if (o > int.MaxValue)
				{
					NB.EncodeInt64(tmp, 0, o);
					@out.Write(tmp, 0, 8);
				}
			}
		}
	}
}
