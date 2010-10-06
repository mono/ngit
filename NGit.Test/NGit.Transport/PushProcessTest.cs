using System;
using System.Collections.Generic;
using NGit;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class PushProcessTest : SampleDataRepositoryTestCase
	{
		private PushProcess process;

		private PushProcessTest.MockTransport transport;

		private HashSet<RemoteRefUpdate> refUpdates;

		private HashSet<Ref> advertisedRefs;

		private RemoteRefUpdate.Status connectionUpdateStatus;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			transport = new PushProcessTest.MockTransport(this, db, new URIish());
			refUpdates = new HashSet<RemoteRefUpdate>();
			advertisedRefs = new HashSet<Ref>();
			connectionUpdateStatus = RemoteRefUpdate.Status.OK;
		}

		/// <summary>Test for fast-forward remote update.</summary>
		/// <remarks>Test for fast-forward remote update.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateFastForward()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.OK, true);
		}

		/// <summary>
		/// Test for non fast-forward remote update, when remote object is not known
		/// to local repository.
		/// </summary>
		/// <remarks>
		/// Test for non fast-forward remote update, when remote object is not known
		/// to local repository.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateNonFastForwardUnknownObject()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("0000000000000000000000000000000000000001"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_NONFASTFORWARD, null
				);
		}

		/// <summary>
		/// Test for non fast-forward remote update, when remote object is known to
		/// local repository, but it is not an ancestor of new object.
		/// </summary>
		/// <remarks>
		/// Test for non fast-forward remote update, when remote object is known to
		/// local repository, but it is not an ancestor of new object.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateNonFastForward()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "ac7e7e44c1885efb472ad54a78327d66bfc4ecef"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_NONFASTFORWARD, null
				);
		}

		/// <summary>Test for non fast-forward remote update, when force update flag is set.</summary>
		/// <remarks>Test for non fast-forward remote update, when force update flag is set.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateNonFastForwardForced()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "ac7e7e44c1885efb472ad54a78327d66bfc4ecef"
				, "refs/heads/master", true, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.OK, false);
		}

		/// <summary>Test for remote ref creation.</summary>
		/// <remarks>Test for remote ref creation.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateCreateRef()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "ac7e7e44c1885efb472ad54a78327d66bfc4ecef"
				, "refs/heads/master", false, null, null);
			TestOneUpdateStatus(rru, null, RemoteRefUpdate.Status.OK, true);
		}

		/// <summary>Test for remote ref deletion.</summary>
		/// <remarks>Test for remote ref deletion.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateDelete()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, null, "refs/heads/master", false, null
				, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.OK, true);
		}

		/// <summary>
		/// Test for remote ref deletion (try), when that ref doesn't exist on remote
		/// repo.
		/// </summary>
		/// <remarks>
		/// Test for remote ref deletion (try), when that ref doesn't exist on remote
		/// repo.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateDeleteNonExisting()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, null, "refs/heads/master", false, null
				, null);
			TestOneUpdateStatus(rru, null, RemoteRefUpdate.Status.NON_EXISTING, null);
		}

		/// <summary>Test for remote ref update, when it is already up to date.</summary>
		/// <remarks>Test for remote ref update, when it is already up to date.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateUpToDate()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.UP_TO_DATE, null);
		}

		/// <summary>Test for remote ref update with expected remote object.</summary>
		/// <remarks>Test for remote ref update with expected remote object.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateExpectedRemote()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, ObjectId.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"
				));
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.OK, true);
		}

		/// <summary>
		/// Test for remote ref update with expected old object set, when old object
		/// is not that expected one.
		/// </summary>
		/// <remarks>
		/// Test for remote ref update with expected old object set, when old object
		/// is not that expected one.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateUnexpectedRemote()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, ObjectId.FromString("0000000000000000000000000000000000000001"
				));
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_REMOTE_CHANGED, null
				);
		}

		/// <summary>
		/// Test for remote ref update with expected old object set, when old object
		/// is not that expected one and force update flag is set (which should have
		/// lower priority) - shouldn't change behavior.
		/// </summary>
		/// <remarks>
		/// Test for remote ref update with expected old object set, when old object
		/// is not that expected one and force update flag is set (which should have
		/// lower priority) - shouldn't change behavior.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateUnexpectedRemoteVsForce()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", true, null, ObjectId.FromString("0000000000000000000000000000000000000001"
				));
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_REMOTE_CHANGED, null
				);
		}

		/// <summary>Test for remote ref update, when connection rejects update.</summary>
		/// <remarks>Test for remote ref update, when connection rejects update.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateRejectedByConnection()
		{
			connectionUpdateStatus = RemoteRefUpdate.Status.REJECTED_OTHER_REASON;
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_OTHER_REASON, null
				);
		}

		/// <summary>
		/// Test for remote refs updates with mixed cases that shouldn't depend on
		/// each other.
		/// </summary>
		/// <remarks>
		/// Test for remote refs updates with mixed cases that shouldn't depend on
		/// each other.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestUpdateMixedCases()
		{
			RemoteRefUpdate rruOk = new RemoteRefUpdate(db, null, "refs/heads/master", false, 
				null, null);
			Ref refToChange = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", 
				ObjectId.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			RemoteRefUpdate rruReject = new RemoteRefUpdate(db, null, "refs/heads/nonexisting"
				, false, null, null);
			refUpdates.AddItem(rruOk);
			refUpdates.AddItem(rruReject);
			advertisedRefs.AddItem(refToChange);
			ExecutePush();
			NUnit.Framework.Assert.AreEqual(RemoteRefUpdate.Status.OK, rruOk.GetStatus());
			NUnit.Framework.Assert.AreEqual(true, rruOk.IsFastForward());
			NUnit.Framework.Assert.AreEqual(RemoteRefUpdate.Status.NON_EXISTING, rruReject.GetStatus
				());
		}

		/// <summary>Test for local tracking ref update.</summary>
		/// <remarks>Test for local tracking ref update.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestTrackingRefUpdateEnabled()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, "refs/remotes/test/master", null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			refUpdates.AddItem(rru);
			advertisedRefs.AddItem(@ref);
			PushResult result = ExecutePush();
			TrackingRefUpdate tru = result.GetTrackingRefUpdate("refs/remotes/test/master");
			NUnit.Framework.Assert.IsNotNull(tru);
			NUnit.Framework.Assert.AreEqual("refs/remotes/test/master", tru.GetLocalName());
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.NEW, tru.GetResult());
		}

		/// <summary>Test for local tracking ref update disabled.</summary>
		/// <remarks>Test for local tracking ref update disabled.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestTrackingRefUpdateDisabled()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			refUpdates.AddItem(rru);
			advertisedRefs.AddItem(@ref);
			PushResult result = ExecutePush();
			NUnit.Framework.Assert.IsTrue(result.GetTrackingRefUpdates().IsEmpty());
		}

		/// <summary>Test for local tracking ref update when remote update has failed.</summary>
		/// <remarks>Test for local tracking ref update when remote update has failed.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestTrackingRefUpdateOnReject()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "ac7e7e44c1885efb472ad54a78327d66bfc4ecef"
				, "refs/heads/master", false, null, null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("2c349335b7f797072cf729c4f3bb0914ecb6dec9"));
			PushResult result = TestOneUpdateStatus(rru, @ref, RemoteRefUpdate.Status.REJECTED_NONFASTFORWARD
				, null);
			NUnit.Framework.Assert.IsTrue(result.GetTrackingRefUpdates().IsEmpty());
		}

		/// <summary>Test for push operation result - that contains expected elements.</summary>
		/// <remarks>Test for push operation result - that contains expected elements.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestPushResult()
		{
			RemoteRefUpdate rru = new RemoteRefUpdate(db, "2c349335b7f797072cf729c4f3bb0914ecb6dec9"
				, "refs/heads/master", false, "refs/remotes/test/master", null);
			Ref @ref = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", ObjectId
				.FromString("ac7e7e44c1885efb472ad54a78327d66bfc4ecef"));
			refUpdates.AddItem(rru);
			advertisedRefs.AddItem(@ref);
			PushResult result = ExecutePush();
			NUnit.Framework.Assert.AreEqual(1, result.GetTrackingRefUpdates().Count);
			NUnit.Framework.Assert.AreEqual(1, result.GetAdvertisedRefs().Count);
			NUnit.Framework.Assert.AreEqual(1, result.GetRemoteUpdates().Count);
			NUnit.Framework.Assert.IsNotNull(result.GetTrackingRefUpdate("refs/remotes/test/master"
				));
			NUnit.Framework.Assert.IsNotNull(result.GetAdvertisedRef("refs/heads/master"));
			NUnit.Framework.Assert.IsNotNull(result.GetRemoteUpdate("refs/heads/master"));
		}

		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="NGit.Errors.TransportException"></exception>
		private PushResult TestOneUpdateStatus(RemoteRefUpdate rru, Ref advertisedRef, RemoteRefUpdate.Status
			 expectedStatus, bool fastForward)
		{
			refUpdates.AddItem(rru);
			if (advertisedRef != null)
			{
				advertisedRefs.AddItem(advertisedRef);
			}
			PushResult result = ExecutePush();
			NUnit.Framework.Assert.AreEqual(expectedStatus, rru.GetStatus());
			if (fastForward != null)
			{
				NUnit.Framework.Assert.AreEqual(fastForward, rru.IsFastForward());
			}
			return result;
		}

		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="NGit.Errors.TransportException"></exception>
		private PushResult ExecutePush()
		{
			process = new PushProcess(transport, refUpdates);
			return process.Execute(new TextProgressMonitor());
		}

		private class MockTransport : NGit.Transport.Transport
		{
			protected MockTransport(PushProcessTest _enclosing, Repository local, URIish uri)
				 : base(local, uri)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.NotSupportedException"></exception>
			/// <exception cref="NGit.Errors.TransportException"></exception>
			public override FetchConnection OpenFetch()
			{
				throw new NotSupportedException("mock");
			}

			/// <exception cref="System.NotSupportedException"></exception>
			/// <exception cref="NGit.Errors.TransportException"></exception>
			public override PushConnection OpenPush()
			{
				return new PushProcessTest.MockPushConnection(this);
			}

			public override void Close()
			{
			}

			private readonly PushProcessTest _enclosing;
			// nothing here
		}

		private class MockPushConnection : BaseConnection, PushConnection
		{
			public MockPushConnection(PushProcessTest _enclosing)
			{
				this._enclosing = _enclosing;
				IDictionary<string, Ref> refsMap = new Dictionary<string, Ref>();
				foreach (Ref r in this._enclosing.advertisedRefs)
				{
					refsMap.Put(r.GetName(), r);
				}
				this.Available(refsMap);
			}

			public override void Close()
			{
			}

			// nothing here
			/// <exception cref="NGit.Errors.TransportException"></exception>
			public virtual void Push(ProgressMonitor monitor, IDictionary<string, RemoteRefUpdate
				> refsToUpdate)
			{
				foreach (RemoteRefUpdate rru in refsToUpdate.Values)
				{
					NUnit.Framework.Assert.AreEqual(RemoteRefUpdate.Status.NOT_ATTEMPTED, rru.GetStatus
						());
					rru.SetStatus(this._enclosing.connectionUpdateStatus);
				}
			}

			private readonly PushProcessTest _enclosing;
		}
	}
}
