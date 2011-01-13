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
using NGit.Notes;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Notes
{
	[NUnit.Framework.TestFixture]
	public class DefaultNoteMergerTest : RepositoryTestCase
	{
		private TestRepository<Repository> tr;

		private ObjectReader reader;

		private ObjectInserter inserter;

		private DefaultNoteMerger merger;

		private Note baseNote;

		private RevBlob noteOn;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			tr = new TestRepository<Repository>(db);
			reader = db.NewObjectReader();
			inserter = db.NewObjectInserter();
			merger = new DefaultNoteMerger();
			noteOn = tr.Blob("a");
			baseNote = NewNote("data");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			reader.Release();
			inserter.Release();
			base.TearDown();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeleteDelete()
		{
			NUnit.Framework.Assert.IsNull(merger.Merge(baseNote, null, null, null, null));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditDelete()
		{
			Note edit = NewNote("edit");
			NUnit.Framework.Assert.AreSame(merger.Merge(baseNote, edit, null, null, null), edit
				);
			NUnit.Framework.Assert.AreSame(merger.Merge(baseNote, null, edit, null, null), edit
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIdenticalEdit()
		{
			Note edit = NewNote("edit");
			NUnit.Framework.Assert.AreSame(merger.Merge(baseNote, edit, edit, null, null), edit
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEditEdit()
		{
			Note edit1 = NewNote("edit1");
			Note edit2 = NewNote("edit2");
			Note result = merger.Merge(baseNote, edit1, edit2, reader, inserter);
			NUnit.Framework.Assert.AreEqual(result, noteOn);
			// same note
			NUnit.Framework.Assert.AreEqual(result.GetData(), tr.Blob("edit1edit2"));
			result = merger.Merge(baseNote, edit2, edit1, reader, inserter);
			NUnit.Framework.Assert.AreEqual(result, noteOn);
			// same note
			NUnit.Framework.Assert.AreEqual(result.GetData(), tr.Blob("edit2edit1"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIdenticalAdd()
		{
			Note add = NewNote("add");
			NUnit.Framework.Assert.AreSame(merger.Merge(null, add, add, null, null), add);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddAdd()
		{
			Note add1 = NewNote("add1");
			Note add2 = NewNote("add2");
			Note result = merger.Merge(null, add1, add2, reader, inserter);
			NUnit.Framework.Assert.AreEqual(result, noteOn);
			// same note
			NUnit.Framework.Assert.AreEqual(result.GetData(), tr.Blob("add1add2"));
			result = merger.Merge(null, add2, add1, reader, inserter);
			NUnit.Framework.Assert.AreEqual(result, noteOn);
			// same note
			NUnit.Framework.Assert.AreEqual(result.GetData(), tr.Blob("add2add1"));
		}

		/// <exception cref="System.Exception"></exception>
		private Note NewNote(string data)
		{
			return new Note(noteOn, tr.Blob(data));
		}
	}
}
