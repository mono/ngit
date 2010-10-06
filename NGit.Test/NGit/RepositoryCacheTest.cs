using NGit;
using NGit.Errors;
using Sharpen;

namespace NGit
{
	public class RepositoryCacheTest : RepositoryTestCase
	{
		public virtual void TestNonBareFileKey()
		{
			FilePath gitdir = db.Directory;
			FilePath parent = gitdir.GetParentFile();
			FilePath other = new FilePath(parent, "notagit");
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Exact(gitdir, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(parent, RepositoryCache.FileKey.Exact(parent, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(other, RepositoryCache.FileKey.Exact(other, db.FileSystem
				).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(gitdir, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(parent, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(other, RepositoryCache.FileKey.Lenient(other, db.
				FileSystem).GetFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestBareFileKey()
		{
			Repository bare = CreateBareRepository();
			FilePath gitdir = bare.Directory;
			FilePath parent = gitdir.GetParentFile();
			string name = gitdir.GetName();
			NUnit.Framework.Assert.IsTrue(name.EndsWith(".git"));
			name = Sharpen.Runtime.Substring(name, 0, name.Length - 4);
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Exact(gitdir, db.
				FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(gitdir, db
				.FileSystem).GetFile());
			NUnit.Framework.Assert.AreEqual(gitdir, RepositoryCache.FileKey.Lenient(new FilePath
				(parent, name), db.FileSystem).GetFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestFileKeyOpenExisting()
		{
			Repository r;
			r = new RepositoryCache.FileKey(db.Directory, db.FileSystem).Open(true);
			NUnit.Framework.Assert.IsNotNull(r);
			NUnit.Framework.Assert.AreEqual(db.Directory, r.Directory);
			r.Close();
			r = new RepositoryCache.FileKey(db.Directory, db.FileSystem).Open(false);
			NUnit.Framework.Assert.IsNotNull(r);
			NUnit.Framework.Assert.AreEqual(db.Directory, r.Directory);
			r.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestFileKeyOpenNew()
		{
			Repository n = CreateBareRepository();
			FilePath gitdir = n.Directory;
			n.Close();
			RecursiveDelete(gitdir);
			NUnit.Framework.Assert.IsFalse(gitdir.Exists());
			try
			{
				new RepositoryCache.FileKey(gitdir, db.FileSystem).Open(true);
				NUnit.Framework.Assert.Fail("incorrectly opened a non existant repository");
			}
			catch (RepositoryNotFoundException e)
			{
				NUnit.Framework.Assert.AreEqual("repository not found: " + gitdir, e.Message);
			}
			Repository o = new RepositoryCache.FileKey(gitdir, db.FileSystem).Open(false);
			NUnit.Framework.Assert.IsNotNull(o);
			NUnit.Framework.Assert.AreEqual(gitdir, o.Directory);
			NUnit.Framework.Assert.IsFalse(gitdir.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCacheRegisterOpen()
		{
			FilePath dir = db.Directory;
			RepositoryCache.Register(db);
			NUnit.Framework.Assert.AreSame(db, RepositoryCache.Open(RepositoryCache.FileKey.Exact
				(dir, db.FileSystem)));
			NUnit.Framework.Assert.AreEqual(".git", dir.GetName());
			FilePath parent = dir.GetParentFile();
			NUnit.Framework.Assert.AreSame(db, RepositoryCache.Open(RepositoryCache.FileKey.Lenient
				(parent, db.FileSystem)));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCacheOpen()
		{
			RepositoryCache.FileKey loc = RepositoryCache.FileKey.Exact(db.Directory, db.FileSystem
				);
			Repository d2 = RepositoryCache.Open(loc);
			NUnit.Framework.Assert.AreNotSame(db, d2);
			NUnit.Framework.Assert.AreSame(d2, RepositoryCache.Open(RepositoryCache.FileKey.Exact
				(loc.GetFile(), db.FileSystem)));
			d2.Close();
			d2.Close();
		}
	}
}
