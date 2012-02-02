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
using NGit.Dircache;
using Sharpen;

namespace NGit.Dircache
{
	[NUnit.Framework.TestFixture]
	public class DirCachePathEditTest
	{
		internal sealed class AddEdit : DirCacheEditor.PathEdit
		{
			public AddEdit(string entryPath) : base(entryPath)
			{
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.REGULAR_FILE;
				ent.SetLength(1);
				ent.SetObjectId(ObjectId.ZeroId);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAddDeletePathAndTreeNormalNames()
		{
			DirCache dc = DirCache.NewInCore();
			DirCacheEditor editor = dc.Editor();
			editor.Add(new DirCachePathEditTest.AddEdit("a"));
			editor.Add(new DirCachePathEditTest.AddEdit("b/c"));
			editor.Add(new DirCachePathEditTest.AddEdit("c/d"));
			editor.Finish();
			NUnit.Framework.Assert.AreEqual(3, dc.GetEntryCount());
			NUnit.Framework.Assert.AreEqual("a", dc.GetEntry(0).PathString);
			NUnit.Framework.Assert.AreEqual("b/c", dc.GetEntry(1).PathString);
			NUnit.Framework.Assert.AreEqual("c/d", dc.GetEntry(2).PathString);
			editor = dc.Editor();
			editor.Add(new DirCacheEditor.DeletePath("b/c"));
			editor.Finish();
			NUnit.Framework.Assert.AreEqual(2, dc.GetEntryCount());
			NUnit.Framework.Assert.AreEqual("a", dc.GetEntry(0).PathString);
			NUnit.Framework.Assert.AreEqual("c/d", dc.GetEntry(1).PathString);
			editor = dc.Editor();
			editor.Add(new DirCacheEditor.DeleteTree(string.Empty));
			editor.Finish();
			NUnit.Framework.Assert.AreEqual(0, dc.GetEntryCount());
		}

		[NUnit.Framework.Test]
		public virtual void TestAddDeleteTrickyNames()
		{
			DirCache dc = DirCache.NewInCore();
			DirCacheEditor editor = dc.Editor();
			editor.Add(new DirCachePathEditTest.AddEdit("a/b"));
			editor.Add(new DirCachePathEditTest.AddEdit("a."));
			editor.Add(new DirCachePathEditTest.AddEdit("ab"));
			editor.Finish();
			NUnit.Framework.Assert.AreEqual(3, dc.GetEntryCount());
			// Validate sort order
			NUnit.Framework.Assert.AreEqual("a.", dc.GetEntry(0).PathString);
			NUnit.Framework.Assert.AreEqual("a/b", dc.GetEntry(1).PathString);
			NUnit.Framework.Assert.AreEqual("ab", dc.GetEntry(2).PathString);
			editor = dc.Editor();
			// Sort order should not confuse DeleteTree
			editor.Add(new DirCacheEditor.DeleteTree("a"));
			editor.Finish();
			NUnit.Framework.Assert.AreEqual(2, dc.GetEntryCount());
			NUnit.Framework.Assert.AreEqual("a.", dc.GetEntry(0).PathString);
			NUnit.Framework.Assert.AreEqual("ab", dc.GetEntry(1).PathString);
		}
	}
}
