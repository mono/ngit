using NGit;
using Sharpen;

namespace NGit
{
	public class TreeIteratorPreOrderTest : RepositoryTestCase
	{
		/// <summary>Empty tree</summary>
		public virtual void TestEmpty()
		{
			Tree tree = new Tree(db);
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>one file</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestSimpleF1()
		{
			Tree tree = new Tree(db);
			tree.AddFile("x");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("x", i.Next().GetName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>two files</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestSimpleF2()
		{
			Tree tree = new Tree(db);
			tree.AddFile("a");
			tree.AddFile("x");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetName());
			NUnit.Framework.Assert.AreEqual("x", i.Next().GetName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <summary>Empty tree</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestSimpleT()
		{
			Tree tree = new Tree(db);
			tree.AddTree("a");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTricky()
		{
			Tree tree = new Tree(db);
			tree.AddFile("a.b");
			tree.AddFile("a.c");
			tree.AddFile("a/b.b/b");
			tree.AddFile("a/b");
			tree.AddFile("a/c");
			tree.AddFile("a=c");
			tree.AddFile("a=d");
			TreeIterator i = MakeIterator(tree);
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual(string.Empty, i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a.b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a.c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b.b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/b.b/b", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a/c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a=c", i.Next().GetFullName());
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreEqual("a=d", i.Next().GetFullName());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		private TreeIterator MakeIterator(Tree tree)
		{
			return new TreeIterator(tree, TreeIterator.Order.PREORDER);
		}
	}
}
