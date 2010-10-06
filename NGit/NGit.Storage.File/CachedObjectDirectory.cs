using System;
using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// The cached instance of an
	/// <see cref="ObjectDirectory">ObjectDirectory</see>
	/// .
	/// <p>
	/// This class caches the list of loose objects in memory, so the file system is
	/// not queried with stat calls.
	/// </summary>
	internal class CachedObjectDirectory : FileObjectDatabase
	{
		/// <summary>
		/// The set that contains unpacked objects identifiers, it is created when
		/// the cached instance is created.
		/// </summary>
		/// <remarks>
		/// The set that contains unpacked objects identifiers, it is created when
		/// the cached instance is created.
		/// </remarks>
		private readonly ObjectIdSubclassMap<ObjectId> unpackedObjects = new ObjectIdSubclassMap
			<ObjectId>();

		private readonly ObjectDirectory wrapped;

		private FileObjectDatabase.AlternateHandle[] alts;

		/// <summary>The constructor</summary>
		/// <param name="wrapped">the wrapped database</param>
		internal CachedObjectDirectory(ObjectDirectory wrapped)
		{
			this.wrapped = wrapped;
			FilePath objects = wrapped.GetDirectory();
			string[] fanout = objects.List();
			if (fanout == null)
			{
				fanout = new string[0];
			}
			foreach (string d in fanout)
			{
				if (d.Length != 2)
				{
					continue;
				}
				string[] entries = new FilePath(objects, d).List();
				if (entries == null)
				{
					continue;
				}
				foreach (string e in entries)
				{
					if (e.Length != Constants.OBJECT_ID_STRING_LENGTH - 2)
					{
						continue;
					}
					try
					{
						unpackedObjects.Add(ObjectId.FromString(d + e));
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		// ignoring the file that does not represent loose object
		public override void Close()
		{
		}

		// Don't close anything.
		public override ObjectInserter NewInserter()
		{
			return ((ObjectDirectoryInserter)wrapped.NewInserter());
		}

		public override ObjectDatabase NewCachedDatabase()
		{
			return this;
		}

		internal override FileObjectDatabase NewCachedFileObjectDatabase()
		{
			return this;
		}

		internal override FilePath GetDirectory()
		{
			return wrapped.GetDirectory();
		}

		internal override FileObjectDatabase.AlternateHandle[] MyAlternates()
		{
			if (alts == null)
			{
				FileObjectDatabase.AlternateHandle[] src = wrapped.MyAlternates();
				alts = new FileObjectDatabase.AlternateHandle[src.Length];
				for (int i = 0; i < alts.Length; i++)
				{
					FileObjectDatabase s = src[i].db;
					alts[i] = new FileObjectDatabase.AlternateHandle(s.NewCachedFileObjectDatabase());
				}
			}
			return alts;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override void Resolve(ICollection<ObjectId> matches, AbbreviatedObjectId
			 id)
		{
			// In theory we could accelerate the loose object scan using our
			// unpackedObjects map, but its not worth the huge code complexity.
			// Scanning a single loose directory is fast enough, and this is
			// unlikely to be called anyway.
			//
			wrapped.Resolve(matches, id);
		}

		internal override bool TryAgain1()
		{
			return wrapped.TryAgain1();
		}

		public override bool Has(AnyObjectId objectId)
		{
			return HasObjectImpl1(objectId);
		}

		internal override bool HasObject1(AnyObjectId objectId)
		{
			return unpackedObjects.Contains(objectId) || wrapped.HasObject1(objectId);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override ObjectLoader OpenObject(WindowCursor curs, AnyObjectId objectId
			)
		{
			return OpenObjectImpl1(curs, objectId);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override ObjectLoader OpenObject1(WindowCursor curs, AnyObjectId objectId
			)
		{
			if (unpackedObjects.Contains(objectId))
			{
				return wrapped.OpenObject2(curs, objectId.Name, objectId);
			}
			return wrapped.OpenObject1(curs, objectId);
		}

		internal override bool HasObject2(string objectId)
		{
			return unpackedObjects.Contains(ObjectId.FromString(objectId));
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override ObjectLoader OpenObject2(WindowCursor curs, string objectName, 
			AnyObjectId objectId)
		{
			if (unpackedObjects.Contains(objectId))
			{
				return wrapped.OpenObject2(curs, objectName, objectId);
			}
			return null;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override long GetObjectSize1(WindowCursor curs, AnyObjectId objectId)
		{
			if (unpackedObjects.Contains(objectId))
			{
				return wrapped.GetObjectSize2(curs, objectId.Name, objectId);
			}
			return wrapped.GetObjectSize1(curs, objectId);
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override long GetObjectSize2(WindowCursor curs, string objectName, AnyObjectId
			 objectId)
		{
			if (unpackedObjects.Contains(objectId))
			{
				return wrapped.GetObjectSize2(curs, objectName, objectId);
			}
			return -1;
		}

		internal override FileObjectDatabase.InsertLooseObjectResult InsertUnpackedObject
			(FilePath tmp, ObjectId objectId, bool createDuplicate)
		{
			FileObjectDatabase.InsertLooseObjectResult result = wrapped.InsertUnpackedObject(
				tmp, objectId, createDuplicate);
			switch (result)
			{
				case FileObjectDatabase.InsertLooseObjectResult.INSERTED:
				case FileObjectDatabase.InsertLooseObjectResult.EXISTS_LOOSE:
				{
					if (!unpackedObjects.Contains(objectId))
					{
						unpackedObjects.Add(objectId);
					}
					break;
				}

				case FileObjectDatabase.InsertLooseObjectResult.EXISTS_PACKED:
				case FileObjectDatabase.InsertLooseObjectResult.FAILURE:
				{
					break;
				}
			}
			return result;
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal override void SelectObjectRepresentation(PackWriter packer, ObjectToPack
			 otp, WindowCursor curs)
		{
			wrapped.SelectObjectRepresentation(packer, otp, curs);
		}
	}
}
