using System.Text;
using Sharpen;

namespace NGit.Util
{
	/// <summary>A rough character sequence around a raw byte buffer.</summary>
	/// <remarks>
	/// A rough character sequence around a raw byte buffer.
	/// <p>
	/// Characters are assumed to be 8-bit US-ASCII.
	/// </remarks>
	public sealed class RawCharSequence : CharSequence
	{
		/// <summary>A zero-length character sequence.</summary>
		/// <remarks>A zero-length character sequence.</remarks>
		public static readonly NGit.Util.RawCharSequence EMPTY = new NGit.Util.RawCharSequence
			(null, 0, 0);

		internal readonly byte[] buffer;

		internal readonly int startPtr;

		internal readonly int endPtr;

		/// <summary>Create a rough character sequence around the raw byte buffer.</summary>
		/// <remarks>Create a rough character sequence around the raw byte buffer.</remarks>
		/// <param name="buf">buffer to scan.</param>
		/// <param name="start">starting position for the sequence.</param>
		/// <param name="end">ending position for the sequence.</param>
		public RawCharSequence(byte[] buf, int start, int end)
		{
			buffer = buf;
			startPtr = start;
			endPtr = end;
		}

		public char CharAt(int index)
		{
			return (char)(buffer[startPtr + index] & unchecked((int)(0xff)));
		}

		public int Length
		{
			get
			{
				return endPtr - startPtr;
			}
		}

		public CharSequence SubSequence(int start, int end)
		{
			return new NGit.Util.RawCharSequence(buffer, startPtr + start, startPtr + end);
		}

		public override string ToString()
		{
			int n = Length;
			StringBuilder b = new StringBuilder(n);
			for (int i = 0; i < n; i++)
			{
				b.Append(CharAt (i));
			}
			return b.ToString();
		}
	}
}
