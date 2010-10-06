using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Visitor for marking all nodes of a tree as modified.</summary>
	/// <remarks>Visitor for marking all nodes of a tree as modified.</remarks>
	public class ForceModified : TreeVisitor
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void StartVisitTree(Tree t)
		{
			t.SetModified();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void EndVisitTree(Tree t)
		{
		}

		// Nothing to do.
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void VisitFile(FileTreeEntry f)
		{
			f.SetModified();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void VisitSymlink(SymlinkTreeEntry s)
		{
		}

		// TODO: handle symlinks. Only problem is that JGit is independent of
		// Eclipse
		// and Pure Java does not know what to do about symbolic links.
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void VisitGitlink(GitlinkTreeEntry s)
		{
		}
		// TODO: handle gitlinks.
	}
}
