using NGit;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class RefSpecTest : TestCase
	{
		public virtual void TestMasterMaster()
		{
			string sn = "refs/heads/master";
			RefSpec rs = new RefSpec(sn + ":" + sn);
			NUnit.Framework.Assert.IsFalse(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetSource());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetDestination());
			NUnit.Framework.Assert.AreEqual(sn + ":" + sn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn, null);
			NUnit.Framework.Assert.IsTrue(rs.MatchSource(r));
			NUnit.Framework.Assert.IsTrue(rs.MatchDestination(r));
			NUnit.Framework.Assert.AreSame(rs, rs.ExpandFromSource(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn + "-and-more", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestSplitLastColon()
		{
			string lhs = ":m:a:i:n:t";
			string rhs = "refs/heads/maint";
			RefSpec rs = new RefSpec(lhs + ":" + rhs);
			NUnit.Framework.Assert.IsFalse(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(lhs, rs.GetSource());
			NUnit.Framework.Assert.AreEqual(rhs, rs.GetDestination());
			NUnit.Framework.Assert.AreEqual(lhs + ":" + rhs, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
		}

		public virtual void TestForceMasterMaster()
		{
			string sn = "refs/heads/master";
			RefSpec rs = new RefSpec("+" + sn + ":" + sn);
			NUnit.Framework.Assert.IsTrue(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetSource());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetDestination());
			NUnit.Framework.Assert.AreEqual("+" + sn + ":" + sn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn, null);
			NUnit.Framework.Assert.IsTrue(rs.MatchSource(r));
			NUnit.Framework.Assert.IsTrue(rs.MatchDestination(r));
			NUnit.Framework.Assert.AreSame(rs, rs.ExpandFromSource(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn + "-and-more", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestMaster()
		{
			string sn = "refs/heads/master";
			RefSpec rs = new RefSpec(sn);
			NUnit.Framework.Assert.IsFalse(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetSource());
			NUnit.Framework.Assert.IsNull(rs.GetDestination());
			NUnit.Framework.Assert.AreEqual(sn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn, null);
			NUnit.Framework.Assert.IsTrue(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
			NUnit.Framework.Assert.AreSame(rs, rs.ExpandFromSource(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn + "-and-more", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestForceMaster()
		{
			string sn = "refs/heads/master";
			RefSpec rs = new RefSpec("+" + sn);
			NUnit.Framework.Assert.IsTrue(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetSource());
			NUnit.Framework.Assert.IsNull(rs.GetDestination());
			NUnit.Framework.Assert.AreEqual("+" + sn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn, null);
			NUnit.Framework.Assert.IsTrue(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
			NUnit.Framework.Assert.AreSame(rs, rs.ExpandFromSource(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn + "-and-more", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestDeleteMaster()
		{
			string sn = "refs/heads/master";
			RefSpec rs = new RefSpec(":" + sn);
			NUnit.Framework.Assert.IsFalse(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.IsNull(rs.GetSource());
			NUnit.Framework.Assert.AreEqual(sn, rs.GetDestination());
			NUnit.Framework.Assert.AreEqual(":" + sn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn, null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsTrue(rs.MatchDestination(r));
			NUnit.Framework.Assert.AreSame(rs, rs.ExpandFromSource(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, sn + "-and-more", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestForceRemotesOrigin()
		{
			string srcn = "refs/heads/*";
			string dstn = "refs/remotes/origin/*";
			RefSpec rs = new RefSpec("+" + srcn + ":" + dstn);
			NUnit.Framework.Assert.IsTrue(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual(srcn, rs.GetSource());
			NUnit.Framework.Assert.AreEqual(dstn, rs.GetDestination());
			NUnit.Framework.Assert.AreEqual("+" + srcn + ":" + dstn, rs.ToString());
			NUnit.Framework.Assert.AreEqual(rs, new RefSpec(rs.ToString()));
			Ref r;
			RefSpec expanded;
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/heads/master", null);
			NUnit.Framework.Assert.IsTrue(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
			expanded = rs.ExpandFromSource(r);
			NUnit.Framework.Assert.AreNotSame(rs, expanded);
			NUnit.Framework.Assert.IsTrue(expanded.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(expanded.IsWildcard());
			NUnit.Framework.Assert.AreEqual(r.GetName(), expanded.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/master", expanded.GetDestination
				());
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/remotes/origin/next", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsTrue(rs.MatchDestination(r));
			r = new ObjectIdRef.Unpeeled(RefStorage.LOOSE, "refs/tags/v1.0", null);
			NUnit.Framework.Assert.IsFalse(rs.MatchSource(r));
			NUnit.Framework.Assert.IsFalse(rs.MatchDestination(r));
		}

		public virtual void TestCreateEmpty()
		{
			RefSpec rs = new RefSpec();
			NUnit.Framework.Assert.IsFalse(rs.IsForceUpdate());
			NUnit.Framework.Assert.IsFalse(rs.IsWildcard());
			NUnit.Framework.Assert.AreEqual("HEAD", rs.GetSource());
			NUnit.Framework.Assert.IsNull(rs.GetDestination());
			NUnit.Framework.Assert.AreEqual("HEAD", rs.ToString());
		}

		public virtual void TestSetForceUpdate()
		{
			string s = "refs/heads/*:refs/remotes/origin/*";
			RefSpec a = new RefSpec(s);
			NUnit.Framework.Assert.IsFalse(a.IsForceUpdate());
			RefSpec b = a.SetForceUpdate(true);
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.IsFalse(a.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(b.IsForceUpdate());
			NUnit.Framework.Assert.AreEqual(s, a.ToString());
			NUnit.Framework.Assert.AreEqual("+" + s, b.ToString());
		}

		public virtual void TestSetSource()
		{
			RefSpec a = new RefSpec();
			RefSpec b = a.SetSource("refs/heads/master");
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.AreEqual("HEAD", a.ToString());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", b.ToString());
		}

		public virtual void TestSetDestination()
		{
			RefSpec a = new RefSpec();
			RefSpec b = a.SetDestination("refs/heads/master");
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.AreEqual("HEAD", a.ToString());
			NUnit.Framework.Assert.AreEqual("HEAD:refs/heads/master", b.ToString());
		}

		public virtual void TestSetDestination_SourceNull()
		{
			RefSpec a = new RefSpec();
			RefSpec b;
			b = a.SetDestination("refs/heads/master");
			b = b.SetSource(null);
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.AreEqual("HEAD", a.ToString());
			NUnit.Framework.Assert.AreEqual(":refs/heads/master", b.ToString());
		}

		public virtual void TestSetSourceDestination()
		{
			RefSpec a = new RefSpec();
			RefSpec b;
			b = a.SetSourceDestination("refs/heads/*", "refs/remotes/origin/*");
			NUnit.Framework.Assert.AreNotSame(a, b);
			NUnit.Framework.Assert.AreEqual("HEAD", a.ToString());
			NUnit.Framework.Assert.AreEqual("refs/heads/*:refs/remotes/origin/*", b.ToString(
				));
		}

		public virtual void TestExpandFromDestination_NonWildcard()
		{
			string src = "refs/heads/master";
			string dst = "refs/remotes/origin/master";
			RefSpec a = new RefSpec(src + ":" + dst);
			RefSpec r = a.ExpandFromDestination(dst);
			NUnit.Framework.Assert.AreSame(a, r);
			NUnit.Framework.Assert.IsFalse(r.IsWildcard());
			NUnit.Framework.Assert.AreEqual(src, r.GetSource());
			NUnit.Framework.Assert.AreEqual(dst, r.GetDestination());
		}

		public virtual void TestExpandFromDestination_Wildcard()
		{
			string src = "refs/heads/master";
			string dst = "refs/remotes/origin/master";
			RefSpec a = new RefSpec("refs/heads/*:refs/remotes/origin/*");
			RefSpec r = a.ExpandFromDestination(dst);
			NUnit.Framework.Assert.AreNotSame(a, r);
			NUnit.Framework.Assert.IsFalse(r.IsWildcard());
			NUnit.Framework.Assert.AreEqual(src, r.GetSource());
			NUnit.Framework.Assert.AreEqual(dst, r.GetDestination());
		}
	}
}
