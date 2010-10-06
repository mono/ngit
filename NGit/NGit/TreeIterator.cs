using System;
using System.IO;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>
	/// A tree iterator iterates over a tree and all its members recursing into
	/// subtrees according to order.
	/// </summary>
	/// <remarks>
	/// A tree iterator iterates over a tree and all its members recursing into
	/// subtrees according to order.
	/// Default is to only visit leafs. An
	/// <see cref="Order">Order</see>
	/// value can be supplied to
	/// make the iteration include Tree nodes as well either before or after the
	/// child nodes have been visited.
	/// </remarks>
	[System.ObsoleteAttribute(@"Use  instead.")]
	public class TreeIterator : Iterator<TreeEntry>
	{
		private Tree tree;

		private int index;

		private NGit.TreeIterator sub;

		private TreeIterator.Order order;

		private bool visitTreeNodes;

		private bool hasVisitedTree;

		/// <summary>Traversal order</summary>
		public enum Order
		{
			PREORDER,
			POSTORDER
		}

		/// <summary>
		/// Construct a
		/// <see cref="TreeIterator">TreeIterator</see>
		/// for visiting all non-tree nodes.
		/// </summary>
		/// <param name="start"></param>
		public TreeIterator(Tree start) : this(start, TreeIterator.Order.PREORDER, false)
		{
		}

		/// <summary>
		/// Construct a
		/// <see cref="TreeIterator">TreeIterator</see>
		/// visiting all nodes in a tree in a given
		/// order.
		/// </summary>
		/// <param name="start">Root node</param>
		/// <param name="order">
		/// 
		/// <see cref="Order">Order</see>
		/// </param>
		public TreeIterator(Tree start, TreeIterator.Order order) : this(start, order, true
			)
		{
		}

		/// <summary>
		/// Construct a
		/// <see cref="TreeIterator">TreeIterator</see>
		/// </summary>
		/// <param name="start">First node to visit</param>
		/// <param name="order">
		/// Visitation
		/// <see cref="Order">Order</see>
		/// </param>
		/// <param name="visitTreeNode">True to include tree node</param>
		private TreeIterator(Tree start, TreeIterator.Order order, bool visitTreeNode)
		{
			this.tree = start;
			this.visitTreeNodes = visitTreeNode;
			this.index = -1;
			this.order = order;
			if (!visitTreeNodes)
			{
				this.hasVisitedTree = true;
			}
			try
			{
				Step();
			}
			catch (IOException e)
			{
				throw new Error(e);
			}
		}

		public override TreeEntry Next()
		{
			try
			{
				TreeEntry ret = NextTreeEntry();
				Step();
				return ret;
			}
			catch (IOException e)
			{
				throw new Error(e);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private TreeEntry NextTreeEntry()
		{
			TreeEntry ret;
			if (sub != null)
			{
				ret = sub.NextTreeEntry();
			}
			else
			{
				if (index < 0 && order == TreeIterator.Order.PREORDER)
				{
					return tree;
				}
				if (order == TreeIterator.Order.POSTORDER && index == tree.MemberCount())
				{
					return tree;
				}
				ret = tree.Members()[index];
			}
			return ret;
		}

		public override bool HasNext()
		{
			try
			{
				return HasNextTreeEntry();
			}
			catch (IOException e)
			{
				throw new Error(e);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private bool HasNextTreeEntry()
		{
			if (tree == null)
			{
				return false;
			}
			return sub != null || index < tree.MemberCount() || order == TreeIterator.Order.POSTORDER
				 && index == tree.MemberCount();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private bool Step()
		{
			if (tree == null)
			{
				return false;
			}
			if (sub != null)
			{
				if (sub.Step())
				{
					return true;
				}
				sub = null;
			}
			if (index < 0 && !hasVisitedTree && order == TreeIterator.Order.PREORDER)
			{
				hasVisitedTree = true;
				return true;
			}
			while (++index < tree.MemberCount())
			{
				TreeEntry e = tree.Members()[index];
				if (e is Tree)
				{
					sub = new NGit.TreeIterator((Tree)e, order, visitTreeNodes);
					if (sub.HasNextTreeEntry())
					{
						return true;
					}
					sub = null;
					continue;
				}
				return true;
			}
			if (index == tree.MemberCount() && !hasVisitedTree && order == TreeIterator.Order
				.POSTORDER)
			{
				hasVisitedTree = true;
				return true;
			}
			return false;
		}

		public override void Remove()
		{
			throw new InvalidOperationException(JGitText.Get().treeIteratorDoesNotSupportRemove
				);
		}
	}
}
