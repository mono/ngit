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
