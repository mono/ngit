using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Abstraction of arbitrary object storage.</summary>
	/// <remarks>
	/// Abstraction of arbitrary object storage.
	/// <p>
	/// An object database stores one or more Git objects, indexed by their unique
	/// <see cref="ObjectId">ObjectId</see>
	/// .
	/// </remarks>
	public abstract class ObjectDatabase
	{
		/// <summary>Initialize a new database instance for access.</summary>
		/// <remarks>Initialize a new database instance for access.</remarks>
		public ObjectDatabase()
		{
		}

		// Protected to force extension.
		/// <summary>Does this database exist yet?</summary>
		/// <returns>
		/// true if this database is already created; false if the caller
		/// should invoke
		/// <see cref="Create()">Create()</see>
		/// to create this database location.
		/// </returns>
		public virtual bool Exists()
		{
			return true;
		}

		/// <summary>Initialize a new object database at this location.</summary>
		/// <remarks>Initialize a new object database at this location.</remarks>
		/// <exception cref="System.IO.IOException">the database could not be created.</exception>
		public virtual void Create()
		{
		}

		// Assume no action is required.
		/// <summary>
		/// Create a new
		/// <code>ObjectInserter</code>
		/// to insert new objects.
		/// <p>
		/// The returned inserter is not itself thread-safe, but multiple concurrent
		/// inserter instances created from the same
		/// <code>ObjectDatabase</code>
		/// must be
		/// thread-safe.
		/// </summary>
		/// <returns>writer the caller can use to create objects in this database.</returns>
		public abstract ObjectInserter NewInserter();

		/// <summary>
		/// Create a new
		/// <code>ObjectReader</code>
		/// to read existing objects.
		/// <p>
		/// The returned reader is not itself thread-safe, but multiple concurrent
		/// reader instances created from the same
		/// <code>ObjectDatabase</code>
		/// must be
		/// thread-safe.
		/// </summary>
		/// <returns>reader the caller can use to load objects from this database.</returns>
		public abstract ObjectReader NewReader();

		/// <summary>Close any resources held by this database.</summary>
		/// <remarks>Close any resources held by this database.</remarks>
		public abstract void Close();

		/// <summary>
		/// Does the requested object exist in this database?
		/// <p>
		/// This is a one-shot call interface which may be faster than allocating a
		/// <see cref="NewReader()">NewReader()</see>
		/// to perform the lookup.
		/// </summary>
		/// <param name="objectId">identity of the object to test for existence of.</param>
		/// <returns>true if the specified object is stored in this database.</returns>
		/// <exception cref="System.IO.IOException">the object store cannot be accessed.</exception>
		public virtual bool Has(AnyObjectId objectId)
		{
			ObjectReader or = NewReader();
			try
			{
				return or.Has(objectId);
			}
			finally
			{
				or.Release();
			}
		}

		/// <summary>Open an object from this database.</summary>
		/// <remarks>
		/// Open an object from this database.
		/// <p>
		/// This is a one-shot call interface which may be faster than allocating a
		/// <see cref="NewReader()">NewReader()</see>
		/// to perform the lookup.
		/// </remarks>
		/// <param name="objectId">identity of the object to open.</param>
		/// <returns>
		/// a
		/// <see cref="ObjectLoader">ObjectLoader</see>
		/// for accessing the object.
		/// </returns>
		/// <exception cref="NGit.Errors.MissingObjectException">the object does not exist.</exception>
		/// <exception cref="System.IO.IOException">the object store cannot be accessed.</exception>
		public virtual ObjectLoader Open(AnyObjectId objectId)
		{
			return Open(objectId, ObjectReader.OBJ_ANY);
		}

		/// <summary>Open an object from this database.</summary>
		/// <remarks>
		/// Open an object from this database.
		/// <p>
		/// This is a one-shot call interface which may be faster than allocating a
		/// <see cref="NewReader()">NewReader()</see>
		/// to perform the lookup.
		/// </remarks>
		/// <param name="objectId">identity of the object to open.</param>
		/// <param name="typeHint">
		/// hint about the type of object being requested;
		/// <see cref="ObjectReader.OBJ_ANY">ObjectReader.OBJ_ANY</see>
		/// if the object type is not known,
		/// or does not matter to the caller.
		/// </param>
		/// <returns>
		/// a
		/// <see cref="ObjectLoader">ObjectLoader</see>
		/// for accessing the object.
		/// </returns>
		/// <exception cref="NGit.Errors.MissingObjectException">the object does not exist.</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">
		/// typeHint was not OBJ_ANY, and the object's actual type does
		/// not match typeHint.
		/// </exception>
		/// <exception cref="System.IO.IOException">the object store cannot be accessed.</exception>
		public virtual ObjectLoader Open(AnyObjectId objectId, int typeHint)
		{
			ObjectReader or = NewReader();
			try
			{
				return or.Open(objectId, typeHint);
			}
			finally
			{
				or.Release();
			}
		}

		/// <summary>Create a new cached database instance over this database.</summary>
		/// <remarks>
		/// Create a new cached database instance over this database. This instance might
		/// optimize queries by caching some information about database. So some modifications
		/// done after instance creation might fail to be noticed.
		/// </remarks>
		/// <returns>new cached database instance</returns>
		public virtual NGit.ObjectDatabase NewCachedDatabase()
		{
			return this;
		}
	}
}
