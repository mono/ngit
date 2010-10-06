using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Abstract TreeVisitor for visiting all files known by a Tree.</summary>
	/// <remarks>Abstract TreeVisitor for visiting all files known by a Tree.</remarks>
	[System.ObsoleteAttribute(@"Use  instead, with a as one of its members.")]
	public abstract class TreeVisitorWithCurrentDirectory : TreeVisitor
	{
		private readonly AList<FilePath> stack = new AList<FilePath>(16);

		private FilePath currentDirectory;

		internal TreeVisitorWithCurrentDirectory(FilePath rootDirectory)
		{
			currentDirectory = rootDirectory;
		}

		internal virtual FilePath GetCurrentDirectory()
		{
			return currentDirectory;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void StartVisitTree(Tree t)
		{
			stack.AddItem(currentDirectory);
			if (!t.IsRoot())
			{
				currentDirectory = new FilePath(currentDirectory, t.GetName());
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void EndVisitTree(Tree t)
		{
			currentDirectory = stack.Remove(stack.Count - 1);
		}

		public abstract void VisitFile(FileTreeEntry arg1);

		public abstract void VisitGitlink(GitlinkTreeEntry arg1);

		public abstract void VisitSymlink(SymlinkTreeEntry arg1);
	}
}
