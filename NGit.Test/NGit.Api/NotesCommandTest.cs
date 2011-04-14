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

using System.Collections.Generic;
using NGit;
using NGit.Api;
using NGit.Notes;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class NotesCommandTest : RepositoryTestCase
	{
		private Git git;

		private RevCommit commit1;

		private RevCommit commit2;

		private static readonly string FILE = "test.txt";

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			// commit something
			WriteTrashFile(FILE, "Hello world");
			git.Add().AddFilepattern(FILE).Call();
			commit1 = git.Commit().SetMessage("Initial commit").Call();
			git.Rm().AddFilepattern(FILE).Call();
			commit2 = git.Commit().SetMessage("Removed file").Call();
			git.NotesAdd().SetObjectId(commit1).SetMessage("data").Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestListNotes()
		{
			IList<Note> notes = git.NotesList().Call();
			NUnit.Framework.Assert.IsTrue(notes.Count == 1);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddAndRemoveNote()
		{
			git.NotesAdd().SetObjectId(commit2).SetMessage("data").Call();
			Note note = git.NotesShow().SetObjectId(commit2).Call();
			string content = Sharpen.Runtime.GetStringForBytes(db.Open(note.GetData()).GetCachedBytes
				(), "UTF-8");
			NUnit.Framework.Assert.AreEqual(content, "data");
			git.NotesRemove().SetObjectId(commit2).Call();
			IList<Note> notes = git.NotesList().Call();
			NUnit.Framework.Assert.IsTrue(notes.Count == 1);
		}
	}
}
