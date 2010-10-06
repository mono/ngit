using NGit;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk
{
	public class TreeWalkBasicDiffTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestMissingSubtree_DetectFileAdded_FileModified()
		{
			ObjectInserter inserter = db.NewObjectInserter();
			ObjectId aFileId = inserter.Insert(Constants.OBJ_BLOB, Constants.Encode("a"));
			ObjectId bFileId = inserter.Insert(Constants.OBJ_BLOB, Constants.Encode("b"));
			ObjectId cFileId1 = inserter.Insert(Constants.OBJ_BLOB, Constants.Encode("c-1"));
			ObjectId cFileId2 = inserter.Insert(Constants.OBJ_BLOB, Constants.Encode("c-2"));
			// Create sub-a/empty, sub-c/empty = hello.
			ObjectId oldTree;
			{
				Tree root = new Tree(db);
				{
					Tree subA = root.AddTree("sub-a");
					subA.AddFile("empty").SetId(aFileId);
					subA.SetId(inserter.Insert(Constants.OBJ_TREE, subA.Format()));
				}
				{
					Tree subC = root.AddTree("sub-c");
					subC.AddFile("empty").SetId(cFileId1);
					subC.SetId(inserter.Insert(Constants.OBJ_TREE, subC.Format()));
				}
				oldTree = inserter.Insert(Constants.OBJ_TREE, root.Format());
			}
			// Create sub-a/empty, sub-b/empty, sub-c/empty.
			ObjectId newTree;
			{
				Tree root = new Tree(db);
				{
					Tree subA = root.AddTree("sub-a");
					subA.AddFile("empty").SetId(aFileId);
					subA.SetId(inserter.Insert(Constants.OBJ_TREE, subA.Format()));
				}
				{
					Tree subB = root.AddTree("sub-b");
					subB.AddFile("empty").SetId(bFileId);
					subB.SetId(inserter.Insert(Constants.OBJ_TREE, subB.Format()));
				}
				{
					Tree subC = root.AddTree("sub-c");
					subC.AddFile("empty").SetId(cFileId2);
					subC.SetId(inserter.Insert(Constants.OBJ_TREE, subC.Format()));
				}
				newTree = inserter.Insert(Constants.OBJ_TREE, root.Format());
			}
			inserter.Flush();
			inserter.Release();
			TreeWalk tw = new TreeWalk(db);
			tw.Reset(new ObjectId[] { oldTree, newTree });
			tw.Recursive = true;
			tw.Filter = TreeFilter.ANY_DIFF;
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("sub-b/empty", tw.PathString);
			NUnit.Framework.Assert.AreEqual(FileMode.MISSING, tw.GetFileMode(0));
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE, tw.GetFileMode(1));
			AssertEquals(ObjectId.ZeroId, tw.GetObjectId(0));
			AssertEquals(bFileId, tw.GetObjectId(1));
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual("sub-c/empty", tw.PathString);
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE, tw.GetFileMode(0));
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE, tw.GetFileMode(1));
			AssertEquals(cFileId1, tw.GetObjectId(0));
			AssertEquals(cFileId2, tw.GetObjectId(1));
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}
	}
}
