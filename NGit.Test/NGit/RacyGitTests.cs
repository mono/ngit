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
using NGit.Treewalk;
using Sharpen;

namespace NGit
{
	public class RacyGitTests : RepositoryTestCase
	{
		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.Exception"></exception>
		public virtual void TestIterator()
		{
			TreeSet<long> modTimes = new TreeSet<long>();
			FilePath lastFile = null;
			for (int i = 0; i < 10; i++)
			{
				lastFile = new FilePath(db.WorkTree, "0." + i);
				lastFile.CreateNewFile();
				if (i == 5)
				{
					FsTick(lastFile);
				}
			}
			modTimes.AddItem(FsTick(lastFile));
			for (int i_1 = 0; i_1 < 10; i_1++)
			{
				lastFile = new FilePath(db.WorkTree, "1." + i_1);
				lastFile.CreateNewFile();
			}
			modTimes.AddItem(FsTick(lastFile));
			for (int i_2 = 0; i_2 < 10; i_2++)
			{
				lastFile = new FilePath(db.WorkTree, "2." + i_2);
				lastFile.CreateNewFile();
				if (i_2 % 4 == 0)
				{
					FsTick(lastFile);
				}
			}
			FileTreeIteratorWithTimeControl fileIt = new FileTreeIteratorWithTimeControl(db, 
				modTimes);
			NameConflictTreeWalk tw = new NameConflictTreeWalk(db);
			tw.Reset();
			tw.AddTree(fileIt);
			tw.Recursive = true;
			FileTreeIterator t;
			long t0 = 0;
			for (int i_3 = 0; i_3 < 10; i_3++)
			{
				NUnit.Framework.Assert.IsTrue(tw.Next());
				t = tw.GetTree<FileTreeIterator>(0);
				if (i_3 == 0)
				{
					t0 = t.GetEntryLastModified();
				}
				else
				{
					NUnit.Framework.Assert.AreEqual(t0, t.GetEntryLastModified());
				}
			}
			long t1 = 0;
			for (int i_4 = 0; i_4 < 10; i_4++)
			{
				NUnit.Framework.Assert.IsTrue(tw.Next());
				t = tw.GetTree<FileTreeIterator>(0);
				if (i_4 == 0)
				{
					t1 = t.GetEntryLastModified();
					NUnit.Framework.Assert.IsTrue(t1 > t0);
				}
				else
				{
					NUnit.Framework.Assert.AreEqual(t1, t.GetEntryLastModified());
				}
			}
			long t2 = 0;
			for (int i_5 = 0; i_5 < 10; i_5++)
			{
				NUnit.Framework.Assert.IsTrue(tw.Next());
				t = tw.GetTree<FileTreeIterator>(0);
				if (i_5 == 0)
				{
					t2 = t.GetEntryLastModified();
					NUnit.Framework.Assert.IsTrue(t2 > t1);
				}
				else
				{
					NUnit.Framework.Assert.AreEqual(t2, t.GetEntryLastModified());
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.Exception"></exception>
		public virtual void TestRacyGitDetection()
		{
			TreeSet<long> modTimes = new TreeSet<long>();
			FilePath lastFile;
			// wait to ensure that modtimes of the file doesn't match last index
			// file modtime
			modTimes.AddItem(FsTick(db.GetIndexFile()));
			// create two files
			AddToWorkDir("a", "a");
			lastFile = AddToWorkDir("b", "b");
			// wait to ensure that file-modTimes and therefore index entry modTime
			// doesn't match the modtime of index-file after next persistance
			modTimes.AddItem(FsTick(lastFile));
			// now add both files to the index. No racy git expected
			ResetIndex(new FileTreeIteratorWithTimeControl(db, modTimes));
			NUnit.Framework.Assert.AreEqual("[a, mode:100644, time:t0, length:1, content:a]" 
				+ "[b, mode:100644, time:t0, length:1, content:b]", IndexState(SMUDGE | MOD_TIME
				 | LENGTH | CONTENT));
			// Remember the last modTime of index file. All modifications times of
			// further modification are translated to this value so it looks that
			// files have been modified in the same time slot as the index file
			modTimes.AddItem(Sharpen.Extensions.ValueOf(db.GetIndexFile().LastModified()));
			// modify one file
			AddToWorkDir("a", "a2");
			// now update the index the index. 'a' has to be racily clean -- because
			// it's modification time is exactly the same as the previous index file
			// mod time.
			ResetIndex(new FileTreeIteratorWithTimeControl(db, modTimes));
			db.ReadDirCache();
			// although racily clean a should not be reported as being dirty
			NUnit.Framework.Assert.AreEqual("[a, mode:100644, time:t1, smudged, length:0, content:a2]"
				 + "[b, mode:100644, time:t0, length:1, content:b]", IndexState(SMUDGE | MOD_TIME
				 | LENGTH | CONTENT));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private FilePath AddToWorkDir(string path, string content)
		{
			FilePath f = new FilePath(db.WorkTree, path);
			FileOutputStream fos = new FileOutputStream(f);
			try
			{
				fos.Write(Sharpen.Runtime.GetBytesForString(content, Constants.CHARACTER_ENCODING
					));
				return f;
			}
			finally
			{
				fos.Close();
			}
		}
	}
}
