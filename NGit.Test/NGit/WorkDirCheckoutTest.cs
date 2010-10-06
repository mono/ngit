using NGit;
using NGit.Errors;
using Sharpen;

namespace NGit
{
	public class WorkDirCheckoutTest : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestFindingConflicts()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("bar", "bar"));
			index.Add(trash, WriteTrashFile("foo/bar/baz/qux", "foo/bar"));
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			WriteTrashFile("bar/baz/qux/foo", "another nasty one");
			WriteTrashFile("foo", "troublesome little bugger");
			WorkDirCheckout workDirCheckout = new WorkDirCheckout(db, trash, index, index);
			workDirCheckout.PrescanOneTree();
			AList<string> conflictingEntries = workDirCheckout.GetConflicts();
			AList<string> removedEntries = workDirCheckout.GetRemoved();
			NUnit.Framework.Assert.AreEqual("bar/baz/qux/foo", conflictingEntries[0]);
			NUnit.Framework.Assert.AreEqual("foo", conflictingEntries[1]);
			GitIndex index2 = new GitIndex(db);
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			index2.Add(trash, WriteTrashFile("bar/baz/qux/foo", "bar"));
			index2.Add(trash, WriteTrashFile("foo", "lalala"));
			workDirCheckout = new WorkDirCheckout(db, trash, index2, index);
			workDirCheckout.PrescanOneTree();
			conflictingEntries = workDirCheckout.GetConflicts();
			removedEntries = workDirCheckout.GetRemoved();
			NUnit.Framework.Assert.IsTrue(conflictingEntries.IsEmpty());
			NUnit.Framework.Assert.IsTrue(removedEntries.Contains("bar/baz/qux/foo"));
			NUnit.Framework.Assert.IsTrue(removedEntries.Contains("foo"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestCheckingOutWithConflicts()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("bar", "bar"));
			index.Add(trash, WriteTrashFile("foo/bar/baz/qux", "foo/bar"));
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			WriteTrashFile("bar/baz/qux/foo", "another nasty one");
			WriteTrashFile("foo", "troublesome little bugger");
			try
			{
				WorkDirCheckout workDirCheckout = new WorkDirCheckout(db, trash, index, index);
				workDirCheckout.Checkout();
				NUnit.Framework.Assert.Fail("Should have thrown exception");
			}
			catch (CheckoutConflictException)
			{
			}
			// all is well
			WorkDirCheckout workDirCheckout_1 = new WorkDirCheckout(db, trash, index, index);
			workDirCheckout_1.SetFailOnConflict(false);
			workDirCheckout_1.Checkout();
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "bar").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar/baz/qux").IsFile());
			GitIndex index2 = new GitIndex(db);
			RecursiveDelete(new FilePath(trash, "bar"));
			RecursiveDelete(new FilePath(trash, "foo"));
			index2.Add(trash, WriteTrashFile("bar/baz/qux/foo", "bar"));
			WriteTrashFile("bar/baz/qux/bar", "evil? I thought it said WEEVIL!");
			index2.Add(trash, WriteTrashFile("foo", "lalala"));
			workDirCheckout_1 = new WorkDirCheckout(db, trash, index2, index);
			workDirCheckout_1.SetFailOnConflict(false);
			workDirCheckout_1.Checkout();
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "bar").IsFile());
			NUnit.Framework.Assert.IsTrue(new FilePath(trash, "foo/bar/baz/qux").IsFile());
			NUnit.Framework.Assert.IsNotNull(index2.GetEntry("bar"));
			NUnit.Framework.Assert.IsNotNull(index2.GetEntry("foo/bar/baz/qux"));
			NUnit.Framework.Assert.IsNull(index2.GetEntry("bar/baz/qux/foo"));
			NUnit.Framework.Assert.IsNull(index2.GetEntry("foo"));
		}
	}
}
