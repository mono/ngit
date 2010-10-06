using System;
using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit;
using NGit.Errors;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// Creates loose objects in a
	/// <see cref="ObjectDirectory">ObjectDirectory</see>
	/// .
	/// </summary>
	internal class ObjectDirectoryInserter : ObjectInserter
	{
		private readonly ObjectDirectory db;

		private readonly Config config;

		private Deflater deflate;

		internal ObjectDirectoryInserter(ObjectDirectory dest, Config cfg)
		{
			db = dest;
			config = cfg;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override ObjectId Insert(int type, long len, InputStream @is)
		{
			MessageDigest md = Digest();
			FilePath tmp = ToTemp(md, type, len, @is);
			ObjectId id = ObjectId.FromRaw(md.Digest());
			switch (db.InsertUnpackedObject(tmp, id, false))
			{
				case FileObjectDatabase.InsertLooseObjectResult.INSERTED:
				case FileObjectDatabase.InsertLooseObjectResult.EXISTS_PACKED:
				case FileObjectDatabase.InsertLooseObjectResult.EXISTS_LOOSE:
				{
					return id;
				}

				case FileObjectDatabase.InsertLooseObjectResult.FAILURE:
				default:
				{
					break;
					break;
				}
			}
			FilePath dst = db.FileFor(id);
			throw new ObjectWritingException("Unable to create new object: " + dst);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Flush()
		{
		}

		// Do nothing. Objects are immediately visible.
		public override void Release()
		{
			if (deflate != null)
			{
				try
				{
					deflate.Finish();
				}
				finally
				{
					deflate = null;
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		/// <exception cref="Sharpen.Error"></exception>
		private FilePath ToTemp(MessageDigest md, int type, long len, InputStream @is)
		{
			bool delete = true;
			FilePath tmp = NewTempFile();
			try
			{
				DigestOutputStream dOut = new DigestOutputStream(Compress(new FileOutputStream(tmp
					)), md);
				try
				{
					WriteHeader(dOut, type, len);
					byte[] buf = Buffer();
					while (len > 0)
					{
						int n = @is.Read(buf, 0, (int)Math.Min(len, buf.Length));
						if (n <= 0)
						{
							throw ShortInput(len);
						}
						dOut.Write(buf, 0, n);
						len -= n;
					}
				}
				finally
				{
					dOut.Close();
				}
				delete = false;
				return tmp;
			}
			finally
			{
				if (delete)
				{
					tmp.Delete();
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void WriteHeader(OutputStream @out, int type, long len)
		{
			@out.Write(Constants.EncodedTypeString(type));
			@out.Write(unchecked((byte)' '));
			@out.Write(Constants.EncodeASCII(len));
			@out.Write(unchecked((byte)0));
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual FilePath NewTempFile()
		{
			return FilePath.CreateTempFile("noz", null, db.GetDirectory());
		}

		internal virtual DeflaterOutputStream Compress(OutputStream @out)
		{
			if (deflate == null)
			{
				deflate = new Deflater(config.Get(CoreConfig.KEY).GetCompression());
			}
			else
			{
				deflate.Reset();
			}
			return new DeflaterOutputStream(@out, deflate);
		}

		private static EOFException ShortInput(long missing)
		{
			return new EOFException("Input did not match supplied length. " + missing + " bytes are missing."
				);
		}
	}
}
