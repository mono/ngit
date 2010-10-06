using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Tree-ish is an interface for tree-like Git objects.</summary>
	/// <remarks>Tree-ish is an interface for tree-like Git objects.</remarks>
	[System.ObsoleteAttribute(@"Use  to parse objects and resolve to a .  See the methodNGit.Revwalk.RevWalk.ParseTree(AnyObjectId) ."
		)]
	public interface Treeish
	{
		/// <returns>the id of this tree</returns>
		ObjectId GetTreeId();

		/// <returns>the tree of this tree-ish object</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		Tree GetTree();
	}
}
