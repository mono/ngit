using System.Text;
using NGit;
using NGit.Ignore;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Ignore
{
	/// <summary>Tests ignore node behavior on the local filesystem.</summary>
	/// <remarks>Tests ignore node behavior on the local filesystem.</remarks>
	public class IgnoreNodeTest : RepositoryTestCase
	{
		private static readonly FileMode D = FileMode.TREE;

		private static readonly FileMode F = FileMode.REGULAR_FILE;

		private const bool ignored = true;

		private const bool tracked = false;

		private TreeWalk walk;

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestRules()
		{
			WriteIgnoreFile(".git/info/exclude", "*~", "/out");
			WriteIgnoreFile(".gitignore", "*.o", "/config");
			WriteTrashFile("config/secret", string.Empty);
			WriteTrashFile("mylib.c", string.Empty);
			WriteTrashFile("mylib.c~", string.Empty);
			WriteTrashFile("mylib.o", string.Empty);
			WriteTrashFile("out/object/foo.exe", string.Empty);
			WriteIgnoreFile("src/config/.gitignore", "lex.out");
			WriteTrashFile("src/config/lex.out", string.Empty);
			WriteTrashFile("src/config/config.c", string.Empty);
			WriteTrashFile("src/config/config.c~", string.Empty);
			WriteTrashFile("src/config/old/lex.out", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, ignored, "config");
			AssertEntry(F, ignored, "config/secret");
			AssertEntry(F, tracked, "mylib.c");
			AssertEntry(F, ignored, "mylib.c~");
			AssertEntry(F, ignored, "mylib.o");
			AssertEntry(D, ignored, "out");
			AssertEntry(D, ignored, "out/object");
			AssertEntry(F, ignored, "out/object/foo.exe");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/config");
			AssertEntry(F, tracked, "src/config/.gitignore");
			AssertEntry(F, tracked, "src/config/config.c");
			AssertEntry(F, ignored, "src/config/config.c~");
			AssertEntry(F, ignored, "src/config/lex.out");
			AssertEntry(D, tracked, "src/config/old");
			AssertEntry(F, ignored, "src/config/old/lex.out");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestNegation()
		{
			WriteIgnoreFile(".gitignore", "*.o");
			WriteIgnoreFile("src/a/b/.gitignore", "!keep.o");
			WriteTrashFile("src/a/b/keep.o", string.Empty);
			WriteTrashFile("src/a/b/nothere.o", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/a");
			AssertEntry(D, tracked, "src/a/b");
			AssertEntry(F, tracked, "src/a/b/.gitignore");
			AssertEntry(F, tracked, "src/a/b/keep.o");
			AssertEntry(F, ignored, "src/a/b/nothere.o");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestSlashOnlyMatchesDirectory()
		{
			WriteIgnoreFile(".gitignore", "out/");
			WriteTrashFile("out", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(F, tracked, "out");
			new FilePath(trash, "out").Delete();
			WriteTrashFile("out/foo", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, ignored, "out");
			AssertEntry(F, ignored, "out/foo");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestWithSlashDoesNotMatchInSubDirectory()
		{
			WriteIgnoreFile(".gitignore", "a/b");
			WriteTrashFile("a/a", string.Empty);
			WriteTrashFile("a/b", string.Empty);
			WriteTrashFile("src/a/a", string.Empty);
			WriteTrashFile("src/a/b", string.Empty);
			BeginWalk();
			AssertEntry(F, tracked, ".gitignore");
			AssertEntry(D, tracked, "a");
			AssertEntry(F, tracked, "a/a");
			AssertEntry(F, ignored, "a/b");
			AssertEntry(D, tracked, "src");
			AssertEntry(D, tracked, "src/a");
			AssertEntry(F, tracked, "src/a/a");
			AssertEntry(F, tracked, "src/a/b");
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		private void BeginWalk()
		{
			walk = new TreeWalk(db);
			walk.Reset();
			walk.AddTree(new FileTreeIterator(db));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertEntry(FileMode type, bool entryIgnored, string pathName)
		{
			NUnit.Framework.Assert.IsTrue("walk has entry", walk.Next());
			NUnit.Framework.Assert.AreEqual(pathName, walk.PathString);
			NUnit.Framework.Assert.AreEqual(type, walk.GetFileMode(0));
			WorkingTreeIterator itr = walk.GetTree<WorkingTreeIterator>(0);
			NUnit.Framework.Assert.IsNotNull("has tree", itr);
			NUnit.Framework.Assert.AreEqual("is ignored", entryIgnored, itr.IsEntryIgnored());
			if (D.Equals(type))
			{
				walk.EnterSubtree();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteIgnoreFile(string name, params string[] rules)
		{
			StringBuilder data = new StringBuilder();
			foreach (string line in rules)
			{
				data.Append(line + "\n");
			}
			WriteTrashFile(name, data.ToString());
		}
	}
}
