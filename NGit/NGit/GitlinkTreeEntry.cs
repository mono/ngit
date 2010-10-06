using System.Text;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>A tree entry representing a gitlink entry used for submodules.</summary>
	/// <remarks>
	/// A tree entry representing a gitlink entry used for submodules.
	/// Note. Java cannot really handle these as file system objects.
	/// </remarks>
	[System.ObsoleteAttribute(@"To look up information about a single path, useNGit.Treewalk.TreeWalk.ForPath(Repository, string, NGit.Revwalk.RevTree) . To lookup information about multiple paths at once, use a and obtain the current entry's information from its getter methods."
		)]
	public class GitlinkTreeEntry : TreeEntry
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct a
		/// <see cref="GitlinkTreeEntry">GitlinkTreeEntry</see>
		/// with the specified name and SHA-1 in
		/// the specified parent
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		/// <param name="nameUTF8"></param>
		protected internal GitlinkTreeEntry(Tree parent, ObjectId id, byte[] nameUTF8) : 
			base(parent, id, nameUTF8)
		{
		}

		public override FileMode GetMode()
		{
			return FileMode.GITLINK;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Accept(TreeVisitor tv, int flags)
		{
			if ((MODIFIED_ONLY & flags) == MODIFIED_ONLY && !IsModified())
			{
				return;
			}
			tv.VisitGitlink(this);
		}

		public override string ToString()
		{
			StringBuilder r = new StringBuilder();
			r.Append(ObjectId.ToString(GetId()));
			r.Append(" G ");
			r.Append(GetFullName());
			return r.ToString();
		}
	}
}
