using System.IO;
using NGit;
using NGit.Storage.File;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Creates the version 1 (old style) pack table of contents files.</summary>
	/// <remarks>Creates the version 1 (old style) pack table of contents files.</remarks>
	/// <seealso cref="PackIndexWriter">PackIndexWriter</seealso>
	/// <seealso cref="PackIndexV1">PackIndexV1</seealso>
	internal class PackIndexWriterV1 : PackIndexWriter
	{
		internal static bool CanStore(PackedObjectInfo oe)
		{
			// We are limited to 4 GB per pack as offset is 32 bit unsigned int.
			//
			return (long)(((ulong)oe.GetOffset()) >> 1) < int.MaxValue;
		}

		protected internal PackIndexWriterV1(OutputStream dst) : base(dst)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override void WriteImpl()
		{
			WriteFanOutTable();
			foreach (PackedObjectInfo oe in entries)
			{
				if (!CanStore(oe))
				{
					throw new IOException(JGitText.Get().packTooLargeForIndexVersion1);
				}
				NB.EncodeInt32(tmp, 0, (int)oe.GetOffset());
				oe.CopyRawTo(tmp, 4);
				@out.Write(tmp);
			}
			WriteChecksumFooter();
		}
	}
}
