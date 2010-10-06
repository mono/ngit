using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Merge;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	public class MergeCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestMergeInItself()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.HEAD)).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.ALREADY_UP_TO_DATE, result.GetMergeStatus
				());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlreadyUpToDate()
		{
			Git git = new Git(db);
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			MergeCommandResult result = git.Merge().Include(db.GetRef("refs/heads/branch1")).
				Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.ALREADY_UP_TO_DATE, result.GetMergeStatus
				());
			AssertEquals(second, result.GetNewHead());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFastForward()
		{
			Git git = new Git(db);
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			CheckoutBranch("refs/heads/branch1");
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.MASTER)).Call
				();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, result.GetMergeStatus()
				);
			AssertEquals(second, result.GetNewHead());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFastForwardWithFiles()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/branch1");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.MASTER)).Call
				();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, result.GetMergeStatus()
				);
			AssertEquals(second, result.GetNewHead());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMultipleHeads()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			WriteTrashFile("file3", "file3");
			git.Add().AddFilepattern("file3").Call();
			git.Commit().SetMessage("third commit").Call();
			CheckoutBranch("refs/heads/branch1");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
			MergeCommand merge = git.Merge();
			merge.Include(second.Id);
			merge.Include(db.GetRef(Constants.MASTER));
			try
			{
				merge.Call();
				NUnit.Framework.Assert.Fail("Expected exception not thrown when merging multiple heads"
					);
			}
			catch (InvalidMergeHeadsException)
			{
			}
		}

		// expected this exception
		/// <exception cref="System.Exception"></exception>
		public virtual void TestContentMerge()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na(main)\n3\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\n<<<<<<< HEAD\na(main)\n=======\na(side)\n>>>>>>> 86503e7e397465588cc267b65d778538bffccb83\n3\n"
				, Read(new FilePath(db.WorkTree, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual(1, result.GetConflicts().Count);
			NUnit.Framework.Assert.AreEqual(3, result.GetConflicts().Get("a")[0].Length);
			NUnit.Framework.Assert.AreEqual(RepositoryState.MERGING, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSuccessfulContentMerge()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			RevCommit thirdCommit = git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1(side)\na\n3(main)\n", Read(new FilePath(db.WorkTree
				, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.IsTrue(2 == result.GetMergedCommits().Length);
			AssertEquals(thirdCommit, result.GetMergedCommits()[0]);
			AssertEquals(secondCommit, result.GetMergedCommits()[1]);
			Iterator<RevCommit> it = git.Log().Call().Iterator();
			RevCommit newHead = it.Next();
			AssertEquals(newHead, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual(2, newHead.ParentCount);
			AssertEquals(thirdCommit, newHead.GetParent(0));
			AssertEquals(secondCommit, newHead.GetParent(1));
			NUnit.Framework.Assert.AreEqual("merging 3fa334456d236a92db020289fe0bf481d91777b4 into HEAD"
				, newHead.GetFullMessage());
			// @TODO fix me
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		// test index state
		/// <exception cref="System.Exception"></exception>
		public virtual void TestSuccessfulContentMergeAndDirtyworkingTree()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("d", "1\nd\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").AddFilepattern
				("d").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			RevCommit thirdCommit = git.Commit().SetMessage("main").Call();
			WriteTrashFile("d", "--- dirty ---");
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1(side)\na\n3(main)\n", Read(new FilePath(db.WorkTree
				, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.IsTrue(2 == result.GetMergedCommits().Length);
			AssertEquals(thirdCommit, result.GetMergedCommits()[0]);
			AssertEquals(secondCommit, result.GetMergedCommits()[1]);
			Iterator<RevCommit> it = git.Log().Call().Iterator();
			RevCommit newHead = it.Next();
			AssertEquals(newHead, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual(2, newHead.ParentCount);
			AssertEquals(thirdCommit, newHead.GetParent(0));
			AssertEquals(secondCommit, newHead.GetParent(1));
			NUnit.Framework.Assert.AreEqual("merging 064d54d98a4cdb0fed1802a21c656bfda67fe879 into HEAD"
				, newHead.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMergeFailingWithDirtyWorkingTree()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			WriteTrashFile("a", "--- dirty ---");
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAILED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("--- dirty ---", Read(new FilePath(db.WorkTree, "a"
				)));
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMergeConflictFileFolder()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("c/c/c", "1\nc(side)\n3\n");
			WriteTrashFile("d", "1\nd(side)\n3\n");
			git.Add().AddFilepattern("c/c/c").AddFilepattern("d").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("c", "1\nc(main)\n3\n");
			WriteTrashFile("d/d/d", "1\nd(main)\n3\n");
			git.Add().AddFilepattern("c").AddFilepattern("d/d/d").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\na\n3\n", Read(new FilePath(db.WorkTree, "a"))
				);
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c")));
			NUnit.Framework.Assert.AreEqual("1\nd(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"d/d/d")));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.AreEqual(RepositoryState.MERGING, db.GetRepositoryState());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void CreateBranch(ObjectId objectId, string branchName)
		{
			RefUpdate updateRef = db.UpdateRef(branchName);
			updateRef.SetNewObjectId(objectId);
			updateRef.Update();
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckoutBranch(string branchName)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			RevCommit branch = walk.ParseCommit(db.Resolve(branchName));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree.Id, db.LockDirCache(), 
				branch.Tree.Id);
			dco.SetFailOnConflict(true);
			dco.Checkout();
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD);
			refUpdate.Link(branchName);
		}
	}
}
