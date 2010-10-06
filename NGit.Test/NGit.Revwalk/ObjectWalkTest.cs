using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class ObjectWalkTest : RevWalkTestCase
	{
		protected internal ObjectWalk objw;

		protected internal override RevWalk CreateRevWalk()
		{
			return objw = new ObjectWalk(db);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNoCommits()
		{
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoCommitsEmptyTree()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			MarkStart(b);
			AssertCommit(b, objw.Next());
			AssertCommit(a, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(Tree(), objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestOneCommitOneTreeTwoBlob()
		{
			RevBlob f0 = Blob("0");
			RevBlob f1 = Blob("1");
			RevTree t = Tree(File("0", f0), File("1", f1), File("2", f1));
			RevCommit a = Commit(t);
			MarkStart(a);
			AssertCommit(a, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(t, objw.NextObject());
			NUnit.Framework.Assert.AreSame(f0, objw.NextObject());
			NUnit.Framework.Assert.AreSame(f1, objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoCommitTwoTreeTwoBlob()
		{
			RevBlob f0 = Blob("0");
			RevBlob f1 = Blob("1");
			RevBlob f2 = Blob("0v2");
			RevTree ta = Tree(File("0", f0), File("1", f1), File("2", f1));
			RevTree tb = Tree(File("0", f2), File("1", f1), File("2", f1));
			RevCommit a = Commit(ta);
			RevCommit b = Commit(tb, a);
			MarkStart(b);
			AssertCommit(b, objw.Next());
			AssertCommit(a, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(tb, objw.NextObject());
			NUnit.Framework.Assert.AreSame(f2, objw.NextObject());
			NUnit.Framework.Assert.AreSame(f1, objw.NextObject());
			NUnit.Framework.Assert.AreSame(ta, objw.NextObject());
			NUnit.Framework.Assert.AreSame(f0, objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoCommitDeepTree1()
		{
			RevBlob f0 = Blob("0");
			RevBlob f1 = Blob("0v2");
			RevTree ta = Tree(File("a/b/0", f0));
			RevTree tb = Tree(File("a/b/1", f1));
			RevCommit a = Commit(ta);
			RevCommit b = Commit(tb, a);
			MarkStart(b);
			AssertCommit(b, objw.Next());
			AssertCommit(a, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(tb, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a/b"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(f1, objw.NextObject());
			NUnit.Framework.Assert.AreSame(ta, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(ta, "a"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(ta, "a/b"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(f0, objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoCommitDeepTree2()
		{
			RevBlob f1 = Blob("1");
			RevTree ta = Tree(File("a/b/0", f1), File("a/c/q", f1));
			RevTree tb = Tree(File("a/b/1", f1), File("a/c/q", f1));
			RevCommit a = Commit(ta);
			RevCommit b = Commit(tb, a);
			MarkStart(b);
			AssertCommit(b, objw.Next());
			AssertCommit(a, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(tb, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a/b"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(f1, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a/c"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(ta, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(ta, "a"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(ta, "a/b"), objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCull()
		{
			RevBlob f1 = Blob("1");
			RevBlob f2 = Blob("2");
			RevBlob f3 = Blob("3");
			RevBlob f4 = Blob("4");
			RevTree ta = Tree(File("a/1", f1), File("c/3", f3));
			RevCommit a = Commit(ta);
			RevTree tb = Tree(File("a/1", f2), File("c/3", f3));
			RevCommit b1 = Commit(tb, a);
			RevCommit b2 = Commit(tb, b1);
			RevTree tc = Tree(File("a/1", f4));
			RevCommit c1 = Commit(tc, a);
			RevCommit c2 = Commit(tc, c1);
			MarkStart(b2);
			MarkUninteresting(c2);
			AssertCommit(b2, objw.Next());
			AssertCommit(b1, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.IsTrue(a.Has(RevFlag.UNINTERESTING));
			NUnit.Framework.Assert.IsTrue(ta.Has(RevFlag.UNINTERESTING));
			NUnit.Framework.Assert.IsTrue(f1.Has(RevFlag.UNINTERESTING));
			NUnit.Framework.Assert.IsTrue(f3.Has(RevFlag.UNINTERESTING));
			NUnit.Framework.Assert.AreSame(tb, objw.NextObject());
			NUnit.Framework.Assert.AreSame(Get(tb, "a"), objw.NextObject());
			NUnit.Framework.Assert.AreSame(f2, objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmptyTreeCorruption()
		{
			ObjectId bId = ObjectId.FromString("abbbfafe3129f85747aba7bfac992af77134c607");
			RevTree tree_root;
			RevTree tree_A;
			RevTree tree_AB;
			RevCommit b;
			{
				Tree root = new Tree(db);
				Tree A = root.AddTree("A");
				FileTreeEntry B = root.AddFile("B");
				B.SetId(bId);
				Tree A_A = A.AddTree("A");
				Tree A_B = A.AddTree("B");
				ObjectInserter inserter = db.NewObjectInserter();
				try
				{
					A_A.SetId(inserter.Insert(Constants.OBJ_TREE, A_A.Format()));
					A_B.SetId(inserter.Insert(Constants.OBJ_TREE, A_B.Format()));
					A.SetId(inserter.Insert(Constants.OBJ_TREE, A.Format()));
					root.SetId(inserter.Insert(Constants.OBJ_TREE, root.Format()));
					inserter.Flush();
				}
				finally
				{
					inserter.Release();
				}
				tree_root = rw.ParseTree(root.GetId());
				tree_A = rw.ParseTree(A.GetId());
				tree_AB = rw.ParseTree(A_A.GetId());
				NUnit.Framework.Assert.AreSame(tree_AB, rw.ParseTree(A_B.GetId()));
				b = Commit(rw.ParseTree(root.GetId()));
			}
			MarkStart(b);
			AssertCommit(b, objw.Next());
			NUnit.Framework.Assert.IsNull(objw.Next());
			NUnit.Framework.Assert.AreSame(tree_root, objw.NextObject());
			NUnit.Framework.Assert.AreSame(tree_A, objw.NextObject());
			NUnit.Framework.Assert.AreSame(tree_AB, objw.NextObject());
			NUnit.Framework.Assert.AreSame(rw.LookupBlob(bId), objw.NextObject());
			NUnit.Framework.Assert.IsNull(objw.NextObject());
		}
	}
}
