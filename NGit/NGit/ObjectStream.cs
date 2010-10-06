using System;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Stream of data coming from an object loaded by
	/// <see cref="ObjectLoader">ObjectLoader</see>
	/// .
	/// </summary>
	public abstract class ObjectStream : InputStream
	{
		/// <returns>
		/// Git object type, see
		/// <see cref="Constants">Constants</see>
		/// .
		/// </returns>
		public abstract int GetType();

		/// <returns>total size of object in bytes</returns>
		public abstract long GetSize();

		/// <summary>Simple stream around the cached byte array created by a loader.</summary>
		/// <remarks>
		/// Simple stream around the cached byte array created by a loader.
		/// <p>
		/// ObjectLoader implementations can use this stream type when the object's
		/// content is small enough to be accessed as a single byte array, but the
		/// application has still requested it in stream format.
		/// </remarks>
		public class SmallStream : ObjectStream
		{
			private readonly int type;

			private readonly byte[] data;

			private int ptr;

			private int mark;

			/// <summary>Create the stream from an existing loader's cached bytes.</summary>
			/// <remarks>Create the stream from an existing loader's cached bytes.</remarks>
			/// <param name="loader">the loader.</param>
			public SmallStream(ObjectLoader loader) : this(loader.GetType(), loader.GetCachedBytes
				())
			{
			}

			/// <summary>Create the stream from an existing byte array and type.</summary>
			/// <remarks>Create the stream from an existing byte array and type.</remarks>
			/// <param name="type">the type constant for the object.</param>
			/// <param name="data">the fully inflated content of the object.</param>
			public SmallStream(int type, byte[] data)
			{
				this.type = type;
				this.data = data;
			}

			public override int GetType()
			{
				return type;
			}

			public override long GetSize()
			{
				return data.Length;
			}

			public override int Available()
			{
				return data.Length - ptr;
			}

			public override long Skip(long n)
			{
				int s = (int)Math.Min(Available(), Math.Max(0, n));
				ptr += s;
				return s;
			}

			public override int Read()
			{
				if (ptr == data.Length)
				{
					return -1;
				}
				return data[ptr++] & unchecked((int)(0xff));
			}

			public override int Read(byte[] b, int off, int len)
			{
				if (ptr == data.Length)
				{
					return -1;
				}
				int n = Math.Min(Available(), len);
				System.Array.Copy(data, ptr, b, off, n);
				ptr += n;
				return n;
			}

			public override bool MarkSupported()
			{
				return true;
			}

			public override void Mark(int readlimit)
			{
				mark = ptr;
			}

			public override void Reset()
			{
				ptr = mark;
			}
		}

		/// <summary>Simple filter stream around another stream.</summary>
		/// <remarks>
		/// Simple filter stream around another stream.
		/// <p>
		/// ObjectLoader implementations can use this stream type when the object's
		/// content is available from a standard InputStream.
		/// </remarks>
		public class Filter : ObjectStream
		{
			private readonly int type;

			private readonly long size;

			private readonly InputStream @in;

			/// <summary>Create a filter stream for an object.</summary>
			/// <remarks>Create a filter stream for an object.</remarks>
			/// <param name="type">the type of the object.</param>
			/// <param name="size">total size of the object, in bytes.</param>
			/// <param name="in">
			/// stream the object's raw data is available from. This
			/// stream should be buffered with some reasonable amount of
			/// buffering.
			/// </param>
			public Filter(int type, long size, InputStream @in)
			{
				this.type = type;
				this.size = size;
				this.@in = @in;
			}

			public override int GetType()
			{
				return type;
			}

			public override long GetSize()
			{
				return size;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override int Available()
			{
				return @in.Available();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override long Skip(long n)
			{
				return @in.Skip(n);
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override int Read()
			{
				return @in.Read();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override int Read(byte[] b, int off, int len)
			{
				return @in.Read(b, off, len);
			}

			public override bool MarkSupported()
			{
				return @in.MarkSupported();
			}

			public override void Mark(int readlimit)
			{
				@in.Mark(readlimit);
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Reset()
			{
				@in.Reset();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Close()
			{
				@in.Close();
			}
		}
	}
}
