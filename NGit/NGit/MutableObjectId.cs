using System;
using NGit;
using NGit.Errors;
using NGit.Util;
using Sharpen;

namespace NGit
{
	/// <summary>A mutable SHA-1 abstraction.</summary>
	/// <remarks>A mutable SHA-1 abstraction.</remarks>
	public class MutableObjectId : AnyObjectId
	{
		/// <summary>Empty constructor.</summary>
		/// <remarks>Empty constructor. Initialize object with default (zeros) value.</remarks>
		public MutableObjectId() : base()
		{
		}

		/// <summary>Copying constructor.</summary>
		/// <remarks>Copying constructor.</remarks>
		/// <param name="src">original entry, to copy id from</param>
		internal MutableObjectId(NGit.MutableObjectId src)
		{
			FromObjectId(src);
		}

		/// <summary>
		/// Make this id match
		/// <see cref="ObjectId.ZeroId()">ObjectId.ZeroId()</see>
		/// .
		/// </summary>
		public virtual void Clear()
		{
			w1 = 0;
			w2 = 0;
			w3 = 0;
			w4 = 0;
			w5 = 0;
		}

		/// <summary>Copy an ObjectId into this mutable buffer.</summary>
		/// <remarks>Copy an ObjectId into this mutable buffer.</remarks>
		/// <param name="src">the source id to copy from.</param>
		public virtual void FromObjectId(AnyObjectId src)
		{
			this.w1 = src.w1;
			this.w2 = src.w2;
			this.w3 = src.w3;
			this.w4 = src.w4;
			this.w5 = src.w5;
		}

		/// <summary>Convert an ObjectId from raw binary representation.</summary>
		/// <remarks>Convert an ObjectId from raw binary representation.</remarks>
		/// <param name="bs">
		/// the raw byte buffer to read from. At least 20 bytes must be
		/// available within this byte array.
		/// </param>
		public virtual void FromRaw(byte[] bs)
		{
			FromRaw(bs, 0);
		}

		/// <summary>Convert an ObjectId from raw binary representation.</summary>
		/// <remarks>Convert an ObjectId from raw binary representation.</remarks>
		/// <param name="bs">
		/// the raw byte buffer to read from. At least 20 bytes after p
		/// must be available within this byte array.
		/// </param>
		/// <param name="p">position to read the first byte of data from.</param>
		public virtual void FromRaw(byte[] bs, int p)
		{
			w1 = NB.DecodeInt32(bs, p);
			w2 = NB.DecodeInt32(bs, p + 4);
			w3 = NB.DecodeInt32(bs, p + 8);
			w4 = NB.DecodeInt32(bs, p + 12);
			w5 = NB.DecodeInt32(bs, p + 16);
		}

		/// <summary>Convert an ObjectId from binary representation expressed in integers.</summary>
		/// <remarks>Convert an ObjectId from binary representation expressed in integers.</remarks>
		/// <param name="ints">
		/// the raw int buffer to read from. At least 5 integers must be
		/// available within this integers array.
		/// </param>
		public virtual void FromRaw(int[] ints)
		{
			FromRaw(ints, 0);
		}

		/// <summary>Convert an ObjectId from binary representation expressed in integers.</summary>
		/// <remarks>Convert an ObjectId from binary representation expressed in integers.</remarks>
		/// <param name="ints">
		/// the raw int buffer to read from. At least 5 integers after p
		/// must be available within this integers array.
		/// </param>
		/// <param name="p">position to read the first integer of data from.</param>
		public virtual void FromRaw(int[] ints, int p)
		{
			w1 = ints[p];
			w2 = ints[p + 1];
			w3 = ints[p + 2];
			w4 = ints[p + 3];
			w5 = ints[p + 4];
		}

		/// <summary>Convert an ObjectId from hex characters (US-ASCII).</summary>
		/// <remarks>Convert an ObjectId from hex characters (US-ASCII).</remarks>
		/// <param name="buf">
		/// the US-ASCII buffer to read from. At least 40 bytes after
		/// offset must be available within this byte array.
		/// </param>
		/// <param name="offset">position to read the first character from.</param>
		public virtual void FromString(byte[] buf, int offset)
		{
			FromHexString(buf, offset);
		}

		/// <summary>Convert an ObjectId from hex characters.</summary>
		/// <remarks>Convert an ObjectId from hex characters.</remarks>
		/// <param name="str">the string to read from. Must be 40 characters long.</param>
		public virtual void FromString(string str)
		{
			if (str.Length != Constants.OBJECT_ID_STRING_LENGTH)
			{
				throw new ArgumentException(MessageFormat.Format(JGitText.Get().invalidId, str));
			}
			FromHexString(Constants.EncodeASCII(str), 0);
		}

		private void FromHexString(byte[] bs, int p)
		{
			try
			{
				w1 = RawParseUtils.ParseHexInt32(bs, p);
				w2 = RawParseUtils.ParseHexInt32(bs, p + 8);
				w3 = RawParseUtils.ParseHexInt32(bs, p + 16);
				w4 = RawParseUtils.ParseHexInt32(bs, p + 24);
				w5 = RawParseUtils.ParseHexInt32(bs, p + 32);
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidObjectIdException(bs, p, Constants.OBJECT_ID_STRING_LENGTH);
			}
		}

		public override ObjectId ToObjectId()
		{
			return new ObjectId(this);
		}
	}
}
