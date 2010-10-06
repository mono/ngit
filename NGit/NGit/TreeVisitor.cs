using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// A TreeVisitor is invoked depth first for every node in a tree and is expected
	/// to perform different actions.
	/// </summary>
	/// <remarks>
	/// A TreeVisitor is invoked depth first for every node in a tree and is expected
	/// to perform different actions.
	/// </remarks>
	[System.ObsoleteAttribute(@"Use  instead.")]
	public interface TreeVisitor
	{
		/// <summary>Visit to a tree node before child nodes are visited.</summary>
		/// <remarks>Visit to a tree node before child nodes are visited.</remarks>
		/// <param name="t">Tree</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void StartVisitTree(Tree t);

		/// <summary>Visit to a tree node.</summary>
		/// <remarks>Visit to a tree node. after child nodes have been visited.</remarks>
		/// <param name="t">Tree</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void EndVisitTree(Tree t);

		/// <summary>Visit to a blob.</summary>
		/// <remarks>Visit to a blob.</remarks>
		/// <param name="f">Blob</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void VisitFile(FileTreeEntry f);

		/// <summary>Visit to a symlink.</summary>
		/// <remarks>Visit to a symlink.</remarks>
		/// <param name="s">Symlink entry</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void VisitSymlink(SymlinkTreeEntry s);

		/// <summary>Visit to a gitlink.</summary>
		/// <remarks>Visit to a gitlink.</remarks>
		/// <param name="s">Gitlink entry</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void VisitGitlink(GitlinkTreeEntry s);
	}
}
