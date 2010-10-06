using System.Collections.Generic;
using NGit;
using NGit.Dircache;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	public class PathSuffixFilterTestCase : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestNonRecursiveFiltering()
		{
			ObjectInserter odi = db.NewObjectInserter();
			ObjectId aSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.sth"));
			ObjectId aTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.txt"));
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder builder = dc.Builder();
			DirCacheEntry aSthEntry = new DirCacheEntry("a.sth");
			aSthEntry.SetFileMode(FileMode.REGULAR_FILE);
			aSthEntry.SetObjectId(aSth);
			DirCacheEntry aTxtEntry = new DirCacheEntry("a.txt");
			aTxtEntry.SetFileMode(FileMode.REGULAR_FILE);
			aTxtEntry.SetObjectId(aTxt);
			builder.Add(aSthEntry);
			builder.Add(aTxtEntry);
			builder.Finish();
			ObjectId treeId = dc.WriteTree(odi);
			odi.Flush();
			TreeWalk tw = new TreeWalk(db);
			tw.Filter = PathSuffixFilter.Create(".txt");
			tw.AddTree(treeId);
			IList<string> paths = new List<string>();
			while (tw.Next())
			{
				paths.AddItem(tw.PathString);
			}
			IList<string> expected = new List<string>();
			expected.AddItem("a.txt");
			NUnit.Framework.Assert.AreEqual(expected, paths);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestRecursiveFiltering()
		{
			ObjectInserter odi = db.NewObjectInserter();
			ObjectId aSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.sth"));
			ObjectId aTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"a.txt"));
			ObjectId bSth = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"b.sth"));
			ObjectId bTxt = odi.Insert(Constants.OBJ_BLOB, Sharpen.Runtime.GetBytesForString(
				"b.txt"));
			DirCache dc = db.ReadDirCache();
			DirCacheBuilder builder = dc.Builder();
			DirCacheEntry aSthEntry = new DirCacheEntry("a.sth");
			aSthEntry.SetFileMode(FileMode.REGULAR_FILE);
			aSthEntry.SetObjectId(aSth);
			DirCacheEntry aTxtEntry = new DirCacheEntry("a.txt");
			aTxtEntry.SetFileMode(FileMode.REGULAR_FILE);
			aTxtEntry.SetObjectId(aTxt);
			builder.Add(aSthEntry);
			builder.Add(aTxtEntry);
			DirCacheEntry bSthEntry = new DirCacheEntry("sub/b.sth");
			bSthEntry.SetFileMode(FileMode.REGULAR_FILE);
			bSthEntry.SetObjectId(bSth);
			DirCacheEntry bTxtEntry = new DirCacheEntry("sub/b.txt");
			bTxtEntry.SetFileMode(FileMode.REGULAR_FILE);
			bTxtEntry.SetObjectId(bTxt);
			builder.Add(bSthEntry);
			builder.Add(bTxtEntry);
			builder.Finish();
			ObjectId treeId = dc.WriteTree(odi);
			odi.Flush();
			TreeWalk tw = new TreeWalk(db);
			tw.Recursive = true;
			tw.Filter = PathSuffixFilter.Create(".txt");
			tw.AddTree(treeId);
			IList<string> paths = new List<string>();
			while (tw.Next())
			{
				paths.AddItem(tw.PathString);
			}
			IList<string> expected = new List<string>();
			expected.AddItem("a.txt");
			expected.AddItem("sub/b.txt");
			NUnit.Framework.Assert.AreEqual(expected, paths);
		}
	}
}
