using System;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Inserts objects into an existing
	/// <code>ObjectDatabase</code>
	/// .
	/// <p>
	/// An inserter is not thread-safe. Individual threads should each obtain their
	/// own unique inserter instance, or must arrange for locking at a higher level
	/// to ensure the inserter is in use by no more than one thread at a time.
	/// <p>
	/// Objects written by an inserter may not be immediately visible for reading
	/// after the insert method completes. Callers must invoke either
	/// <see cref="Release()">Release()</see>
	/// or
	/// <see cref="Flush()">Flush()</see>
	/// prior to updating references or
	/// otherwise making the returned ObjectIds visible to other code.
	/// </summary>
	public abstract class ObjectInserter
	{
		/// <summary>An inserter that can be used for formatting and id generation only.</summary>
		/// <remarks>An inserter that can be used for formatting and id generation only.</remarks>
		public class Formatter : ObjectInserter
		{
			/// <exception cref="System.IO.IOException"></exception>
			public override ObjectId Insert(int objectType, long length, InputStream @in)
			{
				throw new NotSupportedException();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
			}

			// Do nothing.
			public override void Release()
			{
			}
			// Do nothing.
		}

		/// <summary>Digest to compute the name of an object.</summary>
		/// <remarks>Digest to compute the name of an object.</remarks>
		private readonly MessageDigest digest;

		/// <summary>Temporary working buffer for streaming data through.</summary>
		/// <remarks>Temporary working buffer for streaming data through.</remarks>
		private byte[] tempBuffer;

		/// <summary>Create a new inserter for a database.</summary>
		/// <remarks>Create a new inserter for a database.</remarks>
		public ObjectInserter()
		{
			digest = Constants.NewMessageDigest();
		}

		/// <returns>a temporary byte array for use by the caller.</returns>
		protected internal virtual byte[] Buffer()
		{
			if (tempBuffer == null)
			{
				tempBuffer = new byte[8192];
			}
			return tempBuffer;
		}

		/// <returns>digest to help compute an ObjectId</returns>
		protected internal virtual MessageDigest Digest()
		{
			digest.Reset();
			return digest;
		}

		/// <summary>Compute the name of an object, without inserting it.</summary>
		/// <remarks>Compute the name of an object, without inserting it.</remarks>
		/// <param name="type">type code of the object to store.</param>
		/// <param name="data">complete content of the object.</param>
		/// <returns>the name of the object.</returns>
		public virtual ObjectId IdFor(int type, byte[] data)
		{
			return IdFor(type, data, 0, data.Length);
		}

		/// <summary>Compute the name of an object, without inserting it.</summary>
		/// <remarks>Compute the name of an object, without inserting it.</remarks>
		/// <param name="type">type code of the object to store.</param>
		/// <param name="data">complete content of the object.</param>
		/// <param name="off">
		/// first position within
		/// <code>data</code>
		/// .
		/// </param>
		/// <param name="len">
		/// number of bytes to copy from
		/// <code>data</code>
		/// .
		/// </param>
		/// <returns>the name of the object.</returns>
		public virtual ObjectId IdFor(int type, byte[] data, int off, int len)
		{
			MessageDigest md = Digest();
			md.Update(Constants.EncodedTypeString(type));
			md.Update(unchecked((byte)' '));
			md.Update(Constants.EncodeASCII(len));
			md.Update(unchecked((byte)0));
			md.Update(data, off, len);
			return ObjectId.FromRaw(md.Digest());
		}

		/// <summary>Compute the name of an object, without inserting it.</summary>
		/// <remarks>Compute the name of an object, without inserting it.</remarks>
		/// <param name="objectType">type code of the object to store.</param>
		/// <param name="length">
		/// number of bytes to scan from
		/// <code>in</code>
		/// .
		/// </param>
		/// <param name="in">
		/// stream providing the object content. The caller is responsible
		/// for closing the stream.
		/// </param>
		/// <returns>the name of the object.</returns>
		/// <exception cref="System.IO.IOException">the source stream could not be read.</exception>
		public virtual ObjectId IdFor(int objectType, long length, InputStream @in)
		{
			MessageDigest md = Digest();
			md.Update(Constants.EncodedTypeString(objectType));
			md.Update(unchecked((byte)' '));
			md.Update(Constants.EncodeASCII(length));
			md.Update(unchecked((byte)0));
			byte[] buf = Buffer();
			while (length > 0)
			{
				int n = @in.Read(buf, 0, (int)Math.Min(length, buf.Length));
				if (n < 0)
				{
					throw new EOFException("Unexpected end of input");
				}
				md.Update(buf, 0, n);
				length -= n;
			}
			return ObjectId.FromRaw(md.Digest());
		}

		/// <summary>Insert a single commit into the store, returning its unique name.</summary>
		/// <remarks>
		/// Insert a single commit into the store, returning its unique name.
		/// As a side effect,
		/// <see cref="CommitBuilder.CommitId()">CommitBuilder.CommitId()</see>
		/// will also be
		/// populated with the returned ObjectId.
		/// </remarks>
		/// <param name="builder">the builder containing the proposed commit's data.</param>
		/// <returns>the name of the commit object.</returns>
		/// <exception cref="System.IO.IOException">the object could not be stored.</exception>
		public ObjectId Insert(NGit.CommitBuilder builder)
		{
			return Insert(Constants.OBJ_COMMIT, builder.Format(this));
		}

		/// <summary>Insert a single annotated tag into the store, returning its unique name.
		/// 	</summary>
		/// <remarks>
		/// Insert a single annotated tag into the store, returning its unique name.
		/// As a side effect,
		/// <see cref="TagBuilder.GetTagId()">TagBuilder.GetTagId()</see>
		/// will also be populated
		/// with the returned ObjectId.
		/// </remarks>
		/// <param name="builder">the builder containing the proposed tag's data.</param>
		/// <returns>the name of the tag object.</returns>
		/// <exception cref="System.IO.IOException">the object could not be stored.</exception>
		public ObjectId Insert(TagBuilder builder)
		{
			return Insert(Constants.OBJ_TAG, builder.Format(this));
		}

		/// <summary>Insert a single object into the store, returning its unique name.</summary>
		/// <remarks>Insert a single object into the store, returning its unique name.</remarks>
		/// <param name="type">type code of the object to store.</param>
		/// <param name="data">complete content of the object.</param>
		/// <returns>the name of the object.</returns>
		/// <exception cref="System.IO.IOException">the object could not be stored.</exception>
		public virtual ObjectId Insert(int type, byte[] data)
		{
			return Insert(type, data, 0, data.Length);
		}

		/// <summary>Insert a single object into the store, returning its unique name.</summary>
		/// <remarks>Insert a single object into the store, returning its unique name.</remarks>
		/// <param name="type">type code of the object to store.</param>
		/// <param name="data">complete content of the object.</param>
		/// <param name="off">
		/// first position within
		/// <code>data</code>
		/// .
		/// </param>
		/// <param name="len">
		/// number of bytes to copy from
		/// <code>data</code>
		/// .
		/// </param>
		/// <returns>the name of the object.</returns>
		/// <exception cref="System.IO.IOException">the object could not be stored.</exception>
		public virtual ObjectId Insert(int type, byte[] data, int off, int len)
		{
			return Insert(type, len, new ByteArrayInputStream(data, off, len));
		}

		/// <summary>Insert a single object into the store, returning its unique name.</summary>
		/// <remarks>Insert a single object into the store, returning its unique name.</remarks>
		/// <param name="objectType">type code of the object to store.</param>
		/// <param name="length">
		/// number of bytes to copy from
		/// <code>in</code>
		/// .
		/// </param>
		/// <param name="in">
		/// stream providing the object content. The caller is responsible
		/// for closing the stream.
		/// </param>
		/// <returns>the name of the object.</returns>
		/// <exception cref="System.IO.IOException">
		/// the object could not be stored, or the source stream could
		/// not be read.
		/// </exception>
		public abstract ObjectId Insert(int objectType, long length, InputStream @in);

		/// <summary>Make all inserted objects visible.</summary>
		/// <remarks>
		/// Make all inserted objects visible.
		/// <p>
		/// The flush may take some period of time to make the objects available to
		/// other threads.
		/// </remarks>
		/// <exception cref="System.IO.IOException">
		/// the flush could not be completed; objects inserted thus far
		/// are in an indeterminate state.
		/// </exception>
		public abstract void Flush();

		/// <summary>Release any resources used by this inserter.</summary>
		/// <remarks>
		/// Release any resources used by this inserter.
		/// <p>
		/// An inserter that has been released can be used again, but may need to be
		/// released after the subsequent usage.
		/// </remarks>
		public abstract void Release();
	}
}
