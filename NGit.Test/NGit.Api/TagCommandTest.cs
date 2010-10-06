using System;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	public class TagCommandTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		public virtual void TestTaggingOnHead()
		{
			Git git = new Git(db);
			RevCommit commit = git.Commit().SetMessage("initial commit").Call();
			RevTag tag = git.Tag().SetName("tag").Call();
			AssertEquals(commit.Id, tag.GetObject().Id);
		}

		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		public virtual void TestTagging()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			RevCommit commit = git.Commit().SetMessage("second commit").Call();
			git.Commit().SetMessage("third commit").Call();
			RevTag tag = git.Tag().SetObjectId(commit).SetName("tag").Call();
			AssertEquals(commit.Id, tag.GetObject().Id);
		}

		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		public virtual void TestEmptyTagName()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				// forget to tag name
				git.Tag().SetMessage("some message").Call();
				NUnit.Framework.Assert.Fail("We should have failed without a tag name");
			}
			catch (InvalidTagNameException)
			{
			}
		}

		// should hit here
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		public virtual void TestInvalidTagName()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				git.Tag().SetName("bad~tag~name").SetMessage("some message").Call();
				NUnit.Framework.Assert.Fail("We should have failed due to a bad tag name");
			}
			catch (InvalidTagNameException)
			{
			}
		}

		// should hit here
		/// <exception cref="NGit.Api.Errors.NoHeadException"></exception>
		/// <exception cref="NGit.Api.Errors.NoMessageException"></exception>
		/// <exception cref="NGit.Errors.UnmergedPathException"></exception>
		/// <exception cref="NGit.Api.Errors.ConcurrentRefUpdateException"></exception>
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.WrongRepositoryStateException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidTagNameException"></exception>
		public virtual void TestFailureOnSignedTags()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			try
			{
				git.Tag().SetSigned(true).SetName("tag").Call();
				NUnit.Framework.Assert.Fail("We should have failed with an UnsupportedOperationException due to signed tag"
					);
			}
			catch (NotSupportedException)
			{
			}
		}
		// should hit here
	}
}
