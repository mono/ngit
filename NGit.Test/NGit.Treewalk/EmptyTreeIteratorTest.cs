using NGit;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Treewalk
{
	public class EmptyTreeIteratorTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestAtEOF()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCreateSubtreeIterator()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			ObjectReader reader = db.NewObjectReader();
			AbstractTreeIterator sub = etp.CreateSubtreeIterator(reader);
			NUnit.Framework.Assert.IsNotNull(sub);
			NUnit.Framework.Assert.IsTrue(sub.First());
			NUnit.Framework.Assert.IsTrue(sub.Eof());
			NUnit.Framework.Assert.IsTrue(sub is EmptyTreeIterator);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEntryObjectId()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			NUnit.Framework.Assert.AreSame(ObjectId.ZeroId, etp.GetEntryObjectId());
			NUnit.Framework.Assert.IsNotNull(etp.IdBuffer());
			NUnit.Framework.Assert.AreEqual(0, etp.IdOffset());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNextDoesNothing()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			etp.Next(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
			etp.Next(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBackDoesNothing()
		{
			EmptyTreeIterator etp = new EmptyTreeIterator();
			etp.Back(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
			etp.Back(1);
			NUnit.Framework.Assert.IsTrue(etp.First());
			NUnit.Framework.Assert.IsTrue(etp.Eof());
			AssertEquals(ObjectId.ZeroId, ObjectId.FromRaw(etp.IdBuffer()));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestStopWalkCallsParent()
		{
			bool[] called = new bool[1];
			NUnit.Framework.Assert.IsFalse(called[0]);
			EmptyTreeIterator parent = new _EmptyTreeIterator_105(called);
			ObjectReader reader = db.NewObjectReader();
			parent.CreateSubtreeIterator(reader).StopWalk();
			NUnit.Framework.Assert.IsTrue(called[0]);
		}

		private sealed class _EmptyTreeIterator_105 : EmptyTreeIterator
		{
			public _EmptyTreeIterator_105(bool[] called)
			{
				this.called = called;
			}

			public override void StopWalk()
			{
				called[0] = true;
			}

			private readonly bool[] called;
		}
	}
}
