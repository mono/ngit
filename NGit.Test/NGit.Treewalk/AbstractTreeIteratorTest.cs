using NGit;
using NGit.Treewalk;
using NUnit.Framework;
using Sharpen;

namespace NGit.Treewalk
{
	public class AbstractTreeIteratorTest : TestCase
	{
		private static string Prefix(string path)
		{
			int s = path.LastIndexOf('/');
			return s > 0 ? Sharpen.Runtime.Substring(path, 0, s) : string.Empty;
		}

		public class FakeTreeIterator : WorkingTreeIterator
		{
			public FakeTreeIterator(AbstractTreeIteratorTest _enclosing, string pathName, FileMode
				 fileMode) : base(AbstractTreeIteratorTest.Prefix(pathName), new WorkingTreeOptions
				(CoreConfig.AutoCRLF.FALSE))
			{
				this._enclosing = _enclosing;
				this.mode = fileMode.GetBits();
				int s = pathName.LastIndexOf('/');
				byte[] name = Constants.Encode(Sharpen.Runtime.Substring(pathName, s + 1));
				this.EnsurePathCapacity(this.pathOffset + name.Length, this.pathOffset);
				System.Array.Copy(name, 0, this.path, this.pathOffset, name.Length);
				this.pathLen = this.pathOffset + name.Length;
			}

			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override AbstractTreeIterator CreateSubtreeIterator(ObjectReader reader)
			{
				return null;
			}

			private readonly AbstractTreeIteratorTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPathCompare()
		{
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.REGULAR_FILE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a", FileMode.TREE)) < 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.TREE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator(this
				, "a", FileMode.REGULAR_FILE)) > 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.REGULAR_FILE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a", FileMode.REGULAR_FILE)) == 0);
			NUnit.Framework.Assert.IsTrue(new AbstractTreeIteratorTest.FakeTreeIterator(this, 
				"a", FileMode.TREE).PathCompare(new AbstractTreeIteratorTest.FakeTreeIterator(this
				, "a", FileMode.TREE)) == 0);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGrowPath()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			i.GrowPath(2);
			NUnit.Framework.Assert.AreNotSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(origpath.Length * 2, i.path.Length);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEnsurePathCapacityFastCase()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			int want = 50;
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			NUnit.Framework.Assert.IsTrue(want < i.path.Length);
			i.EnsurePathCapacity(want, 2);
			NUnit.Framework.Assert.AreSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEnsurePathCapacityGrows()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "ab", FileMode.TREE);
			int want = 384;
			byte[] origpath = i.path;
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
			NUnit.Framework.Assert.IsTrue(i.path.Length < want);
			i.EnsurePathCapacity(want, 2);
			NUnit.Framework.Assert.AreNotSame(origpath, i.path);
			NUnit.Framework.Assert.AreEqual(512, i.path.Length);
			NUnit.Framework.Assert.AreEqual(i.path[0], 'a');
			NUnit.Framework.Assert.AreEqual(i.path[1], 'b');
		}

		public virtual void TestEntryFileMode()
		{
			foreach (FileMode m in new FileMode[] { FileMode.TREE, FileMode.REGULAR_FILE, FileMode
				.EXECUTABLE_FILE, FileMode.GITLINK, FileMode.SYMLINK })
			{
				AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
					(this, "a", m);
				NUnit.Framework.Assert.AreEqual(m.GetBits(), i.GetEntryRawMode());
				NUnit.Framework.Assert.AreSame(m, i.GetEntryFileMode());
			}
		}

		public virtual void TestEntryPath()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a/b/cd", FileMode.TREE);
			NUnit.Framework.Assert.AreEqual("a/b/cd", i.GetEntryPathString());
			NUnit.Framework.Assert.AreEqual(2, i.GetNameLength());
			byte[] b = new byte[3];
			b[0] = unchecked((int)(0x0a));
			i.GetName(b, 1);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0a)), b[0]);
			NUnit.Framework.Assert.AreEqual('c', b[1]);
			NUnit.Framework.Assert.AreEqual('d', b[2]);
		}

		public virtual void TestCreateEmptyTreeIterator()
		{
			AbstractTreeIteratorTest.FakeTreeIterator i = new AbstractTreeIteratorTest.FakeTreeIterator
				(this, "a/b/cd", FileMode.TREE);
			EmptyTreeIterator e = i.CreateEmptyTreeIterator();
			NUnit.Framework.Assert.IsNotNull(e);
			NUnit.Framework.Assert.AreEqual(i.GetEntryPathString() + "/", e.GetEntryPathString
				());
		}
	}
}
