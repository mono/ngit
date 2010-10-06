using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class IndexTreeWalkerTest : RepositoryTestCase
	{
		private AList<string> treeOnlyEntriesVisited = new AList<string>();

		private AList<string> bothVisited = new AList<string>();

		private AList<string> indexOnlyEntriesVisited = new AList<string>();

		private class TestIndexTreeVisitor : AbstractIndexTreeVisitor
		{
			public override void VisitEntry(TreeEntry treeEntry, GitIndex.Entry indexEntry, FilePath
				 file)
			{
				if (treeEntry == null)
				{
					this._enclosing.indexOnlyEntriesVisited.AddItem(indexEntry.GetName());
				}
				else
				{
					if (indexEntry == null)
					{
						this._enclosing.treeOnlyEntriesVisited.AddItem(treeEntry.GetFullName());
					}
					else
					{
						this._enclosing.bothVisited.AddItem(indexEntry.GetName());
					}
				}
			}

			internal TestIndexTreeVisitor(IndexTreeWalkerTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly IndexTreeWalkerTest _enclosing;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestTreeOnlyOneLevel()
		{
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			tree.AddFile("foo");
			tree.AddFile("bar");
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited[0].Equals("bar"));
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited[1].Equals("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestIndexOnlyOneLevel()
		{
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("foo", "foo"));
			index.Add(trash, WriteTrashFile("bar", "bar"));
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited[0].Equals("bar"));
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited[1].Equals("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestBoth()
		{
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("a", "a"));
			tree.AddFile("b/b");
			index.Add(trash, WriteTrashFile("c", "c"));
			tree.AddFile("c");
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.IsTrue(indexOnlyEntriesVisited.Contains("a"));
			NUnit.Framework.Assert.IsTrue(treeOnlyEntriesVisited.Contains("b/b"));
			NUnit.Framework.Assert.IsTrue(bothVisited.Contains("c"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestIndexOnlySubDirs()
		{
			GitIndex index = new GitIndex(db);
			Tree tree = new Tree(db);
			index.Add(trash, WriteTrashFile("foo/bar/baz", "foobar"));
			index.Add(trash, WriteTrashFile("asdf", "asdf"));
			new IndexTreeWalker(index, tree, trash, new IndexTreeWalkerTest.TestIndexTreeVisitor
				(this)).Walk();
			NUnit.Framework.Assert.AreEqual("asdf", indexOnlyEntriesVisited[0]);
			NUnit.Framework.Assert.AreEqual("foo/bar/baz", indexOnlyEntriesVisited[1]);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestLeavingTree()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("foo/bar", "foo/bar"));
			index.Add(trash, WriteTrashFile("foobar", "foobar"));
			new IndexTreeWalker(index, db.MapTree(index.WriteTree()), trash, new _AbstractIndexTreeVisitor_134
				()).Walk();
		}

		private sealed class _AbstractIndexTreeVisitor_134 : AbstractIndexTreeVisitor
		{
			public _AbstractIndexTreeVisitor_134()
			{
			}

			public override void VisitEntry(TreeEntry entry, GitIndex.Entry indexEntry, FilePath
				 f)
			{
				if (entry == null || indexEntry == null)
				{
					NUnit.Framework.Assert.Fail();
				}
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void FinishVisitTree(Tree tree, int i, string curDir)
			{
				if (tree.MemberCount() == 0)
				{
					NUnit.Framework.Assert.Fail();
				}
				if (i == 0)
				{
					NUnit.Framework.Assert.Fail();
				}
			}
		}
	}
}
