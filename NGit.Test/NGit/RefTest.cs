using System.Collections.Generic;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>Misc tests for refs.</summary>
	/// <remarks>
	/// Misc tests for refs. A lot of things are tested elsewhere so not having a
	/// test for a ref related method, does not mean it is untested.
	/// </remarks>
	public class RefTest : SampleDataRepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		private void WriteSymref(string src, string dst)
		{
			RefUpdate u = db.UpdateRef(src);
			switch (u.Link(dst))
			{
				case RefUpdate.Result.NEW:
				case RefUpdate.Result.FORCED:
				case RefUpdate.Result.NO_CHANGE:
				{
					break;
				}

				default:
				{
					NUnit.Framework.Assert.Fail("link " + src + " to " + dst);
					break;
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestReadAllIncludingSymrefs()
		{
			ObjectId masterId = db.Resolve("refs/heads/master");
			RefUpdate updateRef = db.UpdateRef("refs/remotes/origin/master");
			updateRef.SetNewObjectId(masterId);
			updateRef.SetForceUpdate(true);
			updateRef.Update();
			WriteSymref("refs/remotes/origin/HEAD", "refs/remotes/origin/master");
			ObjectId r = db.Resolve("refs/remotes/origin/HEAD");
			AssertEquals(masterId, r);
			IDictionary<string, Ref> allRefs = db.GetAllRefs();
			Ref refHEAD = allRefs.Get("refs/remotes/origin/HEAD");
			NUnit.Framework.Assert.IsNotNull(refHEAD);
			AssertEquals(masterId, refHEAD.GetObjectId());
			NUnit.Framework.Assert.IsFalse(refHEAD.IsPeeled());
			NUnit.Framework.Assert.IsNull(refHEAD.GetPeeledObjectId());
			Ref refmaster = allRefs.Get("refs/remotes/origin/master");
			AssertEquals(masterId, refmaster.GetObjectId());
			NUnit.Framework.Assert.IsFalse(refmaster.IsPeeled());
			NUnit.Framework.Assert.IsNull(refmaster.GetPeeledObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadSymRefToPacked()
		{
			WriteSymref("HEAD", "refs/heads/b");
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
			NUnit.Framework.Assert.IsTrue("is symref", @ref.IsSymbolic());
			@ref = @ref.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/heads/b", @ref.GetName());
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadSymRefToLoosePacked()
		{
			ObjectId pid = db.Resolve("refs/heads/master^");
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			// internal
			WriteSymref("HEAD", "refs/heads/master");
			Ref @ref = db.GetRef("HEAD");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
			@ref = @ref.GetTarget();
			NUnit.Framework.Assert.AreEqual("refs/heads/master", @ref.GetName());
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestReadLooseRef()
		{
			RefUpdate updateRef = db.UpdateRef("ref/heads/new");
			updateRef.SetNewObjectId(db.Resolve("refs/heads/master"));
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, update);
			Ref @ref = db.GetRef("ref/heads/new");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <summary>Let an "outsider" create a loose ref with the same name as a packed one</summary>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual void TestReadLoosePackedRef()
		{
			Ref @ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
			FileOutputStream os = new FileOutputStream(new FilePath(db.Directory, "refs/heads/master"
				));
			os.Write(Sharpen.Runtime.GetBytesForString(@ref.GetObjectId().Name));
			os.Write('\n');
			os.Close();
			@ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <summary>Modify a packed ref using the API.</summary>
		/// <remarks>
		/// Modify a packed ref using the API. This creates a loose ref too, ie.
		/// LOOSE_PACKED
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestReadSimplePackedRefSameRepo()
		{
			Ref @ref = db.GetRef("refs/heads/master");
			ObjectId pid = db.Resolve("refs/heads/master^");
			NUnit.Framework.Assert.AreEqual(RefStorage.PACKED, @ref.GetStorage());
			RefUpdate updateRef = db.UpdateRef("refs/heads/master");
			updateRef.SetNewObjectId(pid);
			updateRef.SetForceUpdate(true);
			RefUpdate.Result update = updateRef.Update();
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update);
			@ref = db.GetRef("refs/heads/master");
			NUnit.Framework.Assert.AreEqual(RefStorage.LOOSE, @ref.GetStorage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestResolvedNamesBranch()
		{
			Ref @ref = db.GetRef("a");
			NUnit.Framework.Assert.AreEqual("refs/heads/a", @ref.GetName());
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestResolvedSymRef()
		{
			Ref @ref = db.GetRef(Constants.HEAD);
			NUnit.Framework.Assert.AreEqual(Constants.HEAD, @ref.GetName());
			NUnit.Framework.Assert.IsTrue("is symbolic ref", @ref.IsSymbolic());
			NUnit.Framework.Assert.AreSame(RefStorage.LOOSE, @ref.GetStorage());
			Ref dst = @ref.GetTarget();
			NUnit.Framework.Assert.IsNotNull("has target", dst);
			NUnit.Framework.Assert.AreEqual("refs/heads/master", dst.GetName());
			NUnit.Framework.Assert.AreSame(dst.GetObjectId(), @ref.GetObjectId());
			NUnit.Framework.Assert.AreSame(dst.GetPeeledObjectId(), @ref.GetPeeledObjectId());
			NUnit.Framework.Assert.AreEqual(dst.IsPeeled(), @ref.IsPeeled());
		}
	}
}
