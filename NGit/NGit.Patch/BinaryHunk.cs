using NGit;
using NGit.Patch;
using NGit.Util;
using Sharpen;

namespace NGit.Patch
{
	/// <summary>Part of a "GIT binary patch" to describe the pre-image or post-image</summary>
	public class BinaryHunk
	{
		private static readonly byte[] LITERAL = Constants.EncodeASCII("literal ");

		private static readonly byte[] DELTA = Constants.EncodeASCII("delta ");

		/// <summary>Type of information stored in a binary hunk.</summary>
		/// <remarks>Type of information stored in a binary hunk.</remarks>
		public enum Type
		{
			LITERAL_DEFLATED,
			DELTA_DEFLATED
		}

		private readonly FileHeader file;

		/// <summary>
		/// Offset within
		/// <see cref="file">file</see>
		/// .buf to the "literal" or "delta " line.
		/// </summary>
		internal readonly int startOffset;

		/// <summary>
		/// Position 1 past the end of this hunk within
		/// <see cref="file">file</see>
		/// 's buf.
		/// </summary>
		internal int endOffset;

		/// <summary>Type of the data meaning.</summary>
		/// <remarks>Type of the data meaning.</remarks>
		private BinaryHunk.Type type;

		/// <summary>Inflated length of the data.</summary>
		/// <remarks>Inflated length of the data.</remarks>
		private int length;

		internal BinaryHunk(FileHeader fh, int offset)
		{
			file = fh;
			startOffset = offset;
		}

		/// <returns>header for the file this hunk applies to</returns>
		public virtual FileHeader GetFileHeader()
		{
			return file;
		}

		/// <returns>the byte array holding this hunk's patch script.</returns>
		public virtual byte[] GetBuffer()
		{
			return file.buf;
		}

		/// <returns>
		/// offset the start of this hunk in
		/// <see cref="GetBuffer()">GetBuffer()</see>
		/// .
		/// </returns>
		public virtual int GetStartOffset()
		{
			return startOffset;
		}

		/// <returns>
		/// offset one past the end of the hunk in
		/// <see cref="GetBuffer()">GetBuffer()</see>
		/// .
		/// </returns>
		public virtual int GetEndOffset()
		{
			return endOffset;
		}

		/// <returns>type of this binary hunk</returns>
		public virtual BinaryHunk.Type GetType()
		{
			return type;
		}

		/// <returns>inflated size of this hunk's data</returns>
		public virtual int GetSize()
		{
			return length;
		}

		internal virtual int ParseHunk(int ptr, int end)
		{
			byte[] buf = file.buf;
			if (RawParseUtils.Match(buf, ptr, LITERAL) >= 0)
			{
				type = BinaryHunk.Type.LITERAL_DEFLATED;
				length = RawParseUtils.ParseBase10(buf, ptr + LITERAL.Length, null);
			}
			else
			{
				if (RawParseUtils.Match(buf, ptr, DELTA) >= 0)
				{
					type = BinaryHunk.Type.DELTA_DEFLATED;
					length = RawParseUtils.ParseBase10(buf, ptr + DELTA.Length, null);
				}
				else
				{
					// Not a valid binary hunk. Signal to the caller that
					// we cannot parse any further and that this line should
					// be treated otherwise.
					//
					return -1;
				}
			}
			ptr = RawParseUtils.NextLF(buf, ptr);
			// Skip until the first blank line; that is the end of the binary
			// encoded information in this hunk. To save time we don't do a
			// validation of the binary data at this point.
			//
			while (ptr < end)
			{
				bool empty = buf[ptr] == '\n';
				ptr = RawParseUtils.NextLF(buf, ptr);
				if (empty)
				{
					break;
				}
			}
			return ptr;
		}
	}
}
