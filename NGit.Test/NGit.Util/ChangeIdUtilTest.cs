/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using NGit;
using NGit.Junit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Portions of this test is from CommitMsgHookTest in the Android project Gerrit
	/// 	</summary>
	[NUnit.Framework.TestFixture]
	public class ChangeIdUtilTest
	{
		private readonly string SOB1 = "Signed-off-by: J Author <ja@example.com>\n";

		private readonly string SOB2 = "Signed-off-by: J Committer <jc@example.com>\n";

		internal readonly PersonIdent p = RawParseUtils.ParsePersonIdent("A U Thor <author@example.com> 1142878501 -0500"
			);

		internal readonly PersonIdent q = RawParseUtils.ParsePersonIdent("W Riter <writer@example.com> 1142878502 -0500"
			);

		internal ObjectId treeId = ObjectId.FromString("f51de923607cd51cf872b928a6b523ba823f7f35"
			);

		internal ObjectId treeId1 = ObjectId.FromString("4b825dc642cb6eb9a060e54bf8d69288fbee4904"
			);

		internal readonly ObjectId treeId2 = ObjectId.FromString("617601c79811cbbae338512798318b4e5b70c9ac"
			);

		internal ObjectId parentId = ObjectId.FromString("91fea719aaf9447feb9580477eb3dd08b62b5eca"
			);

		internal ObjectId parentId1 = null;

		internal readonly ObjectId parentId2 = ObjectId.FromString("485c91e0600b165c301c278bfbae3e492413980c"
			);

		internal MockSystemReader mockSystemReader = new MockSystemReader();

		internal readonly long when;

		internal readonly int tz;

		internal PersonIdent author = new PersonIdent("J. Author", "ja@example.com");

		internal PersonIdent committer = new PersonIdent("J. Committer", "jc@example.com"
			);

		[NUnit.Framework.Test]
		public virtual void TestClean()
		{
			NUnit.Framework.Assert.AreEqual("hej", ChangeIdUtil.Clean("hej\n\n"));
			NUnit.Framework.Assert.AreEqual("hej\n\nsan", ChangeIdUtil.Clean("hej\n\nsan\n\n"
				));
			NUnit.Framework.Assert.AreEqual("hej\nsan", ChangeIdUtil.Clean("hej\n#men\nsan\n\n#men"
				));
			NUnit.Framework.Assert.AreEqual("hej\nsan", ChangeIdUtil.Clean("hej\nsan\n\n#men"
				));
			NUnit.Framework.Assert.AreEqual("hej\nsan", ChangeIdUtil.Clean("#no\nhej\nsan\n\n#men"
				));
			NUnit.Framework.Assert.AreEqual("hej\nsan", ChangeIdUtil.Clean("#no\nhej\nsan\nSigned-off-by: me \n#men"
				));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestId()
		{
			string msg = "A\nMessage\n";
			ObjectId id = ChangeIdUtil.ComputeChangeId(treeId, parentId, p, q, msg);
			NUnit.Framework.Assert.AreEqual("73f3751208ac92cbb76f9a26ac4a0d9d472e381b", ObjectId
				.ToString(id));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHasChangeid()
		{
			NUnit.Framework.Assert.AreEqual("has changeid\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\nChange-Id: I0123456789012345678901234567890123456789\nAnd then some\n"
				, Call("has changeid\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\nChange-Id: I0123456789012345678901234567890123456789\nAnd then some\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestHasChangeidWithReplacement()
		{
			NUnit.Framework.Assert.AreEqual("has changeid\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\nChange-Id: I988d2d7a6f2c0578fccabd4ebd3cec0768bc7f9f\nAnd then some\n"
				, Call("has changeid\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\nChange-Id: I0123456789012345678901234567890123456789\nAnd then some\n"
				, true));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOneliner()
		{
			NUnit.Framework.Assert.AreEqual("oneliner\n\nChange-Id: I3a98091ce4470de88d52ae317fcd297e2339f063\n"
				, Call("oneliner\n"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestOnelinerFollowedByBlank()
		{
			NUnit.Framework.Assert.AreEqual("oneliner followed by blank\n\nChange-Id: I3a12c21ef342a18498f95c62efbc186cd782b743\n"
				, Call("oneliner followed by blank\n"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestATwoLines()
		{
			NUnit.Framework.Assert.AreEqual("a two lines\nwith text withour break after subject line\n\nChange-Id: I549a0fed3d69b7876c54b4f5a35637135fd43fac\n"
				, Call("a two lines\nwith text withour break after subject line\n"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRegularCommit()
		{
			NUnit.Framework.Assert.AreEqual("regular commit\n\nwith header and body\n\nChange-Id: I62d8749d3c3a888c11e3fadc3924220a19389766\n"
				, Call("regular commit\n\nwith header and body\n"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRegularCommitWithSob_ButNoBody()
		{
			NUnit.Framework.Assert.AreEqual("regular commit with sob, but no body\n\nChange-Id: I0f0b4307e9944ecbd5a9f6b9489e25cfaede43c4\nSigned-off-by: me@you.too\n"
				, Call("regular commit with sob, but no body\n\nSigned-off-by: me@you.too\n"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithBug_SubButNoBody()
		{
			NUnit.Framework.Assert.AreEqual("a commit with bug, sub but no body\n\nBug: 33\nChange-Id: I337e264868613dab6d1e11a34f394db369487412\nSigned-off-by: me@you.too\n"
				, Call("a commit with bug, sub but no body\n\nBug: 33\nSigned-off-by: me@you.too\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubject_NoBodySobAndBug()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject, no body sob and bug\n\nChange-Id: Ib3616d4bf77707a3215a6cb0602c004ee119a445\nSigned-off-by: me@you.too\nBug: 33\n"
				, Call("a commit with subject, no body sob and bug\n\nSigned-off-by: me@you.too\nBug: 33\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubjectBug_NonFooterLineAndSob()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject bug, non-footer line and sob\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\n\nChange-Id: Ia8500eab2304e6e5eac6ae488ff44d5d850d118a\n"
				, Call("a commit with subject bug, non-footer line and sob\n\nBug: 33\nmore text\nSigned-off-by: me@you.too\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubject_NonFooterAndBugAndSob()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject, non-footer and bug and sob\n\nmore text (two empty lines after bug)\nBug: 33\n\n\nChange-Id: Idac75ccbad2ab6727b8612e344df5190d87891dd\nSigned-off-by: me@you.too\n"
				, Call("a commit with subject, non-footer and bug and sob\n\nmore text (two empty lines after bug)\nBug: 33\n\n\nSigned-off-by: me@you.too\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubjectBodyBugBrackersAndSob()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject body, bug. brackers and sob\n\nText\n\nBug: 33\nChange-Id: I90ecb589bef766302532c3e00915e10114b00f62\n[bracket]\nSigned-off-by: me@you.too\n"
				, Call("a commit with subject body, bug. brackers and sob\n\nText\n\nBug: 33\n[bracket]\nSigned-off-by: me@you.too\n\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubjectBodyBugLineWithASpaceAndSob()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject body, bug. line with a space and sob\n\nText\n\nBug: 33\nChange-Id: I864e2218bdee033c8ce9a7f923af9e0d5dc16863\n \nSigned-off-by: me@you.too\n"
				, Call("a commit with subject body, bug. line with a space and sob\n\nText\n\nBug: 33\n \nSigned-off-by: me@you.too\n\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestACommitWithSubjectBodyBugEmptyLineAndSob()
		{
			NUnit.Framework.Assert.AreEqual("a commit with subject body, bug. empty line and sob\n\nText\n\nBug: 33\nChange-Id: I33f119f533313883e6ada3df600c4f0d4db23a76\n \nSigned-off-by: me@you.too\n"
				, Call("a commit with subject body, bug. empty line and sob\n\nText\n\nBug: 33\n \nSigned-off-by: me@you.too\n\n"
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyMessages()
		{
			// Empty input must not produce a change id.
			HookDoesNotModify(string.Empty);
			HookDoesNotModify(" ");
			HookDoesNotModify("\n");
			HookDoesNotModify("\n\n");
			HookDoesNotModify("  \n  ");
			HookDoesNotModify("#");
			HookDoesNotModify("#\n");
			HookDoesNotModify("# on branch master\n# Untracked files:\n");
			HookDoesNotModify("\n# on branch master\n# Untracked files:\n");
			HookDoesNotModify("\n\n# on branch master\n# Untracked files:\n");
			HookDoesNotModify("\n# on branch master\ndiff --git a/src b/src\n" + "new file mode 100644\nindex 0000000..c78b7f0\n"
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestChangeIdAlreadySet()
		{
			// If a Change-Id is already present in the footer, the hook must
			// not modify the message but instead must leave the identity alone.
			//
			HookDoesNotModify("a\n" + "\n" + "Change-Id: Iaeac9b4149291060228ef0154db2985a31111335\n"
				);
			//
			//
			HookDoesNotModify("fix: this thing\n" + "\n" + "Change-Id: I388bdaf52ed05b55e62a22d0a20d2c1ae0d33e7e\n"
				);
			//
			//
			HookDoesNotModify("fix-a-widget: this thing\n" + "\n" + "Change-Id: Id3bc5359d768a6400450283e12bdfb6cd135ea4b\n"
				);
			//
			//
			HookDoesNotModify("FIX: this thing\n" + "\n" + "Change-Id: I1b55098b5a2cce0b3f3da783dda50d5f79f873fa\n"
				);
			//
			//
			HookDoesNotModify("Fix-A-Widget: this thing\n" + "\n" + "Change-Id: I4f4e2e1e8568ddc1509baecb8c1270a1fb4b6da7\n"
				);
		}

		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestChangeIdAlreadySetWithReplacement()
		{
			// If a Change-Id is already present in the footer, the hook
			// replaces the Change-Id with the new value..
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: Ifa324efa85bfb3c8696a46a0f67fa70c35be5f5f\n"
				, Call("a\n" + "\n" + "Change-Id: Iaeac9b4149291060228ef0154db2985a31111335\n", 
				true));
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("fix: this thing\n" + "\n" + "Change-Id: Ib63e4990a06412a3f24bd93bb160e98ac1bd412b\n"
				, Call("fix: this thing\n" + "\n" + "Change-Id: I388bdaf52ed05b55e62a22d0a20d2c1ae0d33e7e\n"
				, true));
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("fix-a-widget: this thing\n" + "\n" + "Change-Id: If0444e4d0cabcf41b3d3b46b7e9a7a64a82117af\n"
				, Call("fix-a-widget: this thing\n" + "\n" + "Change-Id: Id3bc5359d768a6400450283e12bdfb6cd135ea4b\n"
				, true));
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("FIX: this thing\n" + "\n" + "Change-Id: Iba5a3b2d5e5df46448f6daf362b6bfa775c6491d\n"
				, Call("FIX: this thing\n" + "\n" + "Change-Id: I1b55098b5a2cce0b3f3da783dda50d5f79f873fa\n"
				, true));
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("Fix-A-Widget: this thing\n" + "\n" + "Change-Id: I2573d47c62c42429fbe424d70cfba931f8f87848\n"
				, Call("Fix-A-Widget: this thing\n" + "\n" + "Change-Id: I4f4e2e1e8568ddc1509baecb8c1270a1fb4b6da7\n"
				, true));
		}

		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTimeAltersId()
		{
			PersonIdent oldAuthor = author;
			PersonIdent oldCommitter = committer;
			
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				, Call("a\n"));
			//
			//
			//
			Tick();
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I3251906b99dda598a58a6346d8126237ee1ea800\n"
				, Call("a\n"));
			//
			//
			//
			Tick();
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I69adf9208d828f41a3d7e41afbca63aff37c0c5c\n"
				, Call("a\n"));
			
			author = oldAuthor;
			committer = oldCommitter;
		}

		//
		//
		//
		/// <summary>
		/// Increment the
		/// <see cref="author">author</see>
		/// and
		/// <see cref="committer">committer</see>
		/// times.
		/// </summary>
		protected internal virtual void Tick()
		{
			long delta = TimeUnit.MILLISECONDS.Convert(5 * 60, TimeUnit.SECONDS);
			long now = author.GetWhen().GetTime() + delta;
			author = new PersonIdent(author, now, tz);
			committer = new PersonIdent(committer, now, tz);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFirstParentAltersId()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				, Call("a\n"));
			//
			//
			//
			ObjectId old = parentId1;
			parentId1 = parentId2;
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I51e86482bde7f92028541aaf724d3a3f996e7ea2\n"
				, Call("a\n"));
			parentId1 = old;
		}

		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirCacheAltersId()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				, Call("a\n"));
			//
			//
			//
			ObjectId old = treeId1;
			treeId1 = treeId2;
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: If56597ea9759f23b070677ea6f064c60c38da631\n"
				, Call("a\n"));
			treeId1 = old;
		}

		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSingleLineMessages()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				, Call("a\n"));
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("fix: this thing\n" + "\n" + "Change-Id: I0f13d0e6c739ca3ae399a05a93792e80feb97f37\n"
				, Call("fix: this thing\n"));
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("fix-a-widget: this thing\n" + "\n" + "Change-Id: I1a1a0c751e4273d532e4046a501a612b9b8a775e\n"
				, Call("fix-a-widget: this thing\n"));
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("FIX: this thing\n" + "\n" + "Change-Id: If816d944c57d3893b60cf10c65931fead1290d97\n"
				, Call("FIX: this thing\n"));
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("Fix-A-Widget: this thing\n" + "\n" + "Change-Id: I3e18d00cbda2ba1f73aeb63ed8c7d57d7fd16c76\n"
				, Call("Fix-A-Widget: this thing\n"));
		}

		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultiLineMessagesWithoutFooter()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b\n" + "\n" + "Change-Id: Id0b4f42d3d6fc1569595c9b97cb665e738486f5d\n"
				, Call("a\n" + "\n" + "b\n"));
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "Change-Id: I7d237b20058a0f46cc3f5fabc4a0476877289d75\n"
				, Call("a\n" + "\n" + "b\nc\nd\ne\n"));
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n"
				 + "\n" + "Change-Id: I382e662f47bf164d6878b7fe61637873ab7fa4e8\n", Call("a\n" +
				 "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n"));
		}

		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSingleLineMessagesWithSignedOffBy()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				 + SOB1, Call("a\n" + "\n" + SOB1));
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				 + SOB1 + SOB2, Call("a\n" + "\n" + SOB1 + SOB2));
		}

		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultiLineMessagesWithSignedOffBy()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n"
				 + "\n" + "Change-Id: I382e662f47bf164d6878b7fe61637873ab7fa4e8\n" + SOB1, Call(
				"a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n" + "\n" + SOB1));
			//
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n"
				 + "\n" + "Change-Id: I382e662f47bf164d6878b7fe61637873ab7fa4e8\n" + SOB1 + SOB2
				, Call("a\n" + "\n" + "b\nc\nd\ne\n" + "\n" + "f\ng\nh\n" + "\n" + SOB1 + SOB2));
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "b: not a footer\nc\nd\ne\n" + "\n"
				 + "f\ng\nh\n" + "\n" + "Change-Id: I8869aabd44b3017cd55d2d7e0d546a03e3931ee2\n"
				 + SOB1 + SOB2, Call("a\n" + "\n" + "b: not a footer\nc\nd\ne\n" + "\n" + "f\ng\nh\n"
				 + "\n" + SOB1 + SOB2));
		}

		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoteInMiddle()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "NOTE: This\n" + "does not fix it.\n"
				 + "\n" + "Change-Id: I988a127969a6ee5e58db546aab74fc46e66847f8\n", Call("a\n" +
				 "\n" + "NOTE: This\n" + "does not fix it.\n"));
		}

		//
		//
		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestKernelStyleFooter()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I1bd787f9e7590a2ac82b02c404c955ffb21877c4\n"
				 + SOB1 + "[ja: Fixed\n" + "     the indentation]\n" + SOB2, Call("a\n" + "\n" +
				 SOB1 + "[ja: Fixed\n" + "     the indentation]\n" + SOB2));
		}

		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestChangeIdAfterBugOrIssue()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Bug: 42\n" + "Change-Id: I8c0321227c4324e670b9ae8cf40eccc87af21b1b\n"
				 + SOB1, Call("a\n" + "\n" + "Bug: 42\n" + SOB1));
			//
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Issue: 42\n" + "Change-Id: Ie66e07d89ae5b114c0975b49cf326e90331dd822\n"
				 + SOB1, Call("a\n" + "\n" + "Issue: 42\n" + SOB1));
		}

		//
		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		public virtual void NotestCommitDashV()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "Change-Id: I7fc3876fee63c766a2063df97fbe04a2dddd8d7c\n"
				 + SOB1 + SOB2, Call("a\n" + "\n" + SOB1 + SOB2 + "\n" + "# on branch master\n" 
				+ "diff --git a/src b/src\n" + "new file mode 100644\n" + "index 0000000..c78b7f0\n"
				));
		}

		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWithEndingURL()
		{
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "http://example.com/ fixes this\n"
				 + "\n" + "Change-Id: I3b7e4e16b503ce00f07ba6ad01d97a356dad7701\n", Call("a\n" +
				 "\n" + "http://example.com/ fixes this\n"));
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "https://example.com/ fixes this\n"
				 + "\n" + "Change-Id: I62b9039e2fc0dce274af55e8f99312a8a80a805d\n", Call("a\n" +
				 "\n" + "https://example.com/ fixes this\n"));
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "ftp://example.com/ fixes this\n" 
				+ "\n" + "Change-Id: I71b05dc1f6b9a5540a53a693e64d58b65a8910e8\n", Call("a\n" + 
				"\n" + "ftp://example.com/ fixes this\n"));
			//
			//
			//
			//
			//
			//
			//
			NUnit.Framework.Assert.AreEqual("a\n" + "\n" + "git://example.com/ fixes this\n" 
				+ "\n" + "Change-Id: Id34e942baa68d790633737d815ddf11bac9183e5\n", Call("a\n" + 
				"\n" + "git://example.com/ fixes this\n"));
		}

		//
		//
		//
		//
		//
		//
		//
		/// <exception cref="System.Exception"></exception>
		private void HookDoesNotModify(string @in)
		{
			NUnit.Framework.Assert.AreEqual(@in, Call(@in));
		}

		/// <exception cref="System.Exception"></exception>
		private string Call(string body)
		{
			return Call(body, false);
		}

		/// <exception cref="System.Exception"></exception>
		private string Call(string body, bool replaceExisting)
		{
			ObjectId computeChangeId = ChangeIdUtil.ComputeChangeId(treeId1, parentId1, author
				, committer, body);
			if (computeChangeId == null)
			{
				return body;
			}
			return ChangeIdUtil.InsertId(body, computeChangeId, replaceExisting);
		}

		public ChangeIdUtilTest()
		{
			when = mockSystemReader.GetCurrentTime();
			tz = new MockSystemReader().GetTimezone(when);
			{
				author = new PersonIdent(author, when, tz);
			}
			{
				committer = new PersonIdent(committer, when, tz);
			}
		}
	}
}
