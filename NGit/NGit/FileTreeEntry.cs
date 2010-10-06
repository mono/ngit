using System.Text;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// A representation of a file (blob) object in a
	/// <see cref="Tree">Tree</see>
	/// .
	/// </summary>
	[System.ObsoleteAttribute(@"To look up information about a single path, useNGit.Treewalk.TreeWalk.ForPath(Repository, string, NGit.Revwalk.RevTree) . To lookup information about multiple paths at once, use a and obtain the current entry's information from its getter methods."
		)]
	public class FileTreeEntry : TreeEntry
	{
		private FileMode mode;

		/// <summary>Constructor for a File (blob) object.</summary>
		/// <remarks>Constructor for a File (blob) object.</remarks>
		/// <param name="parent">
		/// The
		/// <see cref="Tree">Tree</see>
		/// holding this object (or null)
		/// </param>
		/// <param name="id">the SHA-1 of the blob (or null for a yet unhashed file)</param>
		/// <param name="nameUTF8">raw object name in the parent tree</param>
		/// <param name="execute">true if the executable flag is set</param>
		public FileTreeEntry(Tree parent, ObjectId id, byte[] nameUTF8, bool execute) : base
			(parent, id, nameUTF8)
		{
			SetExecutable(execute);
		}

		public override FileMode GetMode()
		{
			return mode;
		}

		/// <returns>true if this file is executable</returns>
		public virtual bool IsExecutable()
		{
			return GetMode().Equals(FileMode.EXECUTABLE_FILE);
		}

		/// <param name="execute">set/reset the executable flag</param>
		public virtual void SetExecutable(bool execute)
		{
			mode = execute ? FileMode.EXECUTABLE_FILE : FileMode.REGULAR_FILE;
		}

		/// <returns>
		/// an
		/// <see cref="ObjectLoader">ObjectLoader</see>
		/// that will return the data
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual ObjectLoader OpenReader()
		{
			return GetRepository().Open(GetId(), Constants.OBJ_BLOB);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Accept(TreeVisitor tv, int flags)
		{
			if ((MODIFIED_ONLY & flags) == MODIFIED_ONLY && !IsModified())
			{
				return;
			}
			tv.VisitFile(this);
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append(ObjectId.ToString(GetId()));
			r.Append(' ');
			r.Append(IsExecutable() ? 'X' : 'F');
			r.Append(' ');
			r.Append(GetFullName());
			return r.ToString();
		}
	}
}
