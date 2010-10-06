using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// Writes out refs to the
	/// <see cref="Constants.INFO_REFS">Constants.INFO_REFS</see>
	/// and
	/// <see cref="Constants.PACKED_REFS">Constants.PACKED_REFS</see>
	/// files.
	/// This class is abstract as the writing of the files must be handled by the
	/// caller. This is because it is used by transport classes as well.
	/// </summary>
	public abstract class RefWriter
	{
		private readonly ICollection<Ref> refs;

		/// <param name="refs">
		/// the complete set of references. This should have been computed
		/// by applying updates to the advertised refs already discovered.
		/// </param>
		public RefWriter(ICollection<Ref> refs)
		{
			this.refs = RefComparator.Sort(refs);
		}

		/// <param name="refs">
		/// the complete set of references. This should have been computed
		/// by applying updates to the advertised refs already discovered.
		/// </param>
		public RefWriter(IDictionary<string, Ref> refs)
		{
			if (refs is RefMap)
			{
				this.refs = refs.Values;
			}
			else
			{
				this.refs = RefComparator.Sort(refs.Values);
			}
		}

		/// <param name="refs">
		/// the complete set of references. This should have been computed
		/// by applying updates to the advertised refs already discovered.
		/// </param>
		public RefWriter(RefList<Ref> refs)
		{
			this.refs = refs.AsList();
		}

		/// <summary>
		/// Rebuild the
		/// <see cref="Constants.INFO_REFS">Constants.INFO_REFS</see>
		/// .
		/// <p>
		/// This method rebuilds the contents of the
		/// <see cref="Constants.INFO_REFS">Constants.INFO_REFS</see>
		/// file
		/// to match the passed list of references.
		/// </summary>
		/// <exception cref="System.IO.IOException">
		/// writing is not supported, or attempting to write the file
		/// failed, possibly due to permissions or remote disk full, etc.
		/// </exception>
		public virtual void WriteInfoRefs()
		{
			StringWriter w = new StringWriter();
			char[] tmp = new char[Constants.OBJECT_ID_STRING_LENGTH];
			foreach (Ref r in refs)
			{
				if (Constants.HEAD.Equals(r.GetName()))
				{
					// Historically HEAD has never been published through
					// the INFO_REFS file. This is a mistake, but its the
					// way things are.
					//
					continue;
				}
				r.GetObjectId().CopyTo(tmp, w);
				w.Write('\t');
				w.Write(r.GetName());
				w.Write('\n');
				if (r.GetPeeledObjectId() != null)
				{
					r.GetPeeledObjectId().CopyTo(tmp, w);
					w.Write('\t');
					w.Write(r.GetName());
					w.Write("^{}\n");
				}
			}
			WriteFile(Constants.INFO_REFS, Constants.Encode(w.ToString()));
		}

		/// <summary>
		/// Rebuild the
		/// <see cref="Constants.PACKED_REFS">Constants.PACKED_REFS</see>
		/// file.
		/// <p>
		/// This method rebuilds the contents of the
		/// <see cref="Constants.PACKED_REFS">Constants.PACKED_REFS</see>
		/// file to match the passed list of references, including only those refs
		/// that have a storage type of
		/// <see cref="RefStorage.PACKED">RefStorage.PACKED</see>
		/// .
		/// </summary>
		/// <exception cref="System.IO.IOException">
		/// writing is not supported, or attempting to write the file
		/// failed, possibly due to permissions or remote disk full, etc.
		/// </exception>
		public virtual void WritePackedRefs()
		{
			bool peeled = false;
			foreach (Ref r in refs)
			{
				if (r.GetStorage().IsPacked() && r.IsPeeled())
				{
					peeled = true;
					break;
				}
			}
			StringWriter w = new StringWriter();
			if (peeled)
			{
				w.Write(RefDirectory.PACKED_REFS_HEADER);
				if (peeled)
				{
					w.Write(RefDirectory.PACKED_REFS_PEELED);
				}
				w.Write('\n');
			}
			char[] tmp = new char[Constants.OBJECT_ID_STRING_LENGTH];
			foreach (Ref r_1 in refs)
			{
				if (r_1.GetStorage() != RefStorage.PACKED)
				{
					continue;
				}
				r_1.GetObjectId().CopyTo(tmp, w);
				w.Write(' ');
				w.Write(r_1.GetName());
				w.Write('\n');
				if (r_1.GetPeeledObjectId() != null)
				{
					w.Write('^');
					r_1.GetPeeledObjectId().CopyTo(tmp, w);
					w.Write('\n');
				}
			}
			WriteFile(Constants.PACKED_REFS, Constants.Encode(w.ToString()));
		}

		/// <summary>
		/// Handles actual writing of ref files to the git repository, which may
		/// differ slightly depending on the destination and transport.
		/// </summary>
		/// <remarks>
		/// Handles actual writing of ref files to the git repository, which may
		/// differ slightly depending on the destination and transport.
		/// </remarks>
		/// <param name="file">path to ref file.</param>
		/// <param name="content">byte content of file to be written.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		protected internal abstract void WriteFile(string file, byte[] content);
	}
}
