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

using System;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class URIishTest : TestCase
	{
		private static readonly string GIT_SCHEME = "git://";

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUnixFile()
		{
			string str = "/home/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual(str, u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWindowsFile()
		{
			string str = "D:/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual(str, u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWindowsFile2()
		{
			string str = "D:\\m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("D:/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual("D:/m y", u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestRelativePath()
		{
			string str = "../../foo/bar";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual(str, u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUNC()
		{
			string str = "\\\\some\\place";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("//some/place", u.GetPath());
			NUnit.Framework.Assert.AreEqual("//some/place", u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileProtoUnix()
		{
			string str = "file:///home/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("file", u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/home/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileProtoWindows()
		{
			string str = "file:///D:/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("file", u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("D:/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitProtoUnix()
		{
			string str = "git://example.com/home/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual("/home/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitProtoUnixPort()
		{
			string str = "git://example.com:333/home/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual("/home/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual(333, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitProtoWindowsPort()
		{
			string str = "git://example.com:338/D:/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("D:/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual(338, u.GetPort());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitProtoWindows()
		{
			string str = "git://example.com/D:/m y";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("D:/m y", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestScpStyleWithoutUser()
		{
			string str = "example.com:some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestScpStyleWithUser()
		{
			string str = "user@example.com:some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("user", u.GetUser());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitSshProto()
		{
			string str = "git+ssh://example.com/some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git+ssh", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSshGitProto()
		{
			string str = "ssh+git://example.com/some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("ssh+git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSshProto()
		{
			string str = "ssh://example.com/some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("ssh", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSshProtoWithUserAndPort()
		{
			string str = "ssh://user@example.com:33/some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("ssh", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual("user", u.GetUser());
			NUnit.Framework.Assert.IsNull(u.GetPass());
			NUnit.Framework.Assert.AreEqual(33, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSshProtoWithUserPassAndPort()
		{
			string str = "ssh://user:pass@example.com:33/some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("ssh", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.AreEqual("user", u.GetUser());
			NUnit.Framework.Assert.AreEqual("pass", u.GetPass());
			NUnit.Framework.Assert.AreEqual(33, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToPrivateString());
			NUnit.Framework.Assert.AreEqual(u.SetPass(null).ToPrivateString(), u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGitWithUserHome()
		{
			string str = "git://example.com/~some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.AreEqual("git", u.GetScheme());
			NUnit.Framework.Assert.IsTrue(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("~some/p ath", u.GetPath());
			NUnit.Framework.Assert.AreEqual("example.com", u.GetHost());
			NUnit.Framework.Assert.IsNull(u.GetUser());
			NUnit.Framework.Assert.IsNull(u.GetPass());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToPrivateString());
			NUnit.Framework.Assert.AreEqual(u.SetPass(null).ToPrivateString(), u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileWithNoneUserHomeWithTilde()
		{
			string str = "/~some/p ath";
			URIish u = new URIish(str);
			NUnit.Framework.Assert.IsNull(u.GetScheme());
			NUnit.Framework.Assert.IsFalse(u.IsRemote());
			NUnit.Framework.Assert.AreEqual("/~some/p ath", u.GetPath());
			NUnit.Framework.Assert.IsNull(u.GetHost());
			NUnit.Framework.Assert.IsNull(u.GetUser());
			NUnit.Framework.Assert.IsNull(u.GetPass());
			NUnit.Framework.Assert.AreEqual(-1, u.GetPort());
			NUnit.Framework.Assert.AreEqual(str, u.ToPrivateString());
			NUnit.Framework.Assert.AreEqual(u.SetPass(null).ToPrivateString(), u.ToString());
			NUnit.Framework.Assert.AreEqual(u, new URIish(str));
		}

		public virtual void TestGetNullHumanishName()
		{
			try
			{
				new URIish().GetHumanishName();
				NUnit.Framework.Assert.Fail("path must be not null");
			}
			catch (ArgumentException)
			{
			}
		}

		// expected
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetEmptyHumanishName()
		{
			try
			{
				new URIish(GIT_SCHEME).GetHumanishName();
				NUnit.Framework.Assert.Fail("empty path is useless");
			}
			catch (ArgumentException)
			{
			}
		}

		// expected
		public virtual void TestGetAbsEmptyHumanishName()
		{
			try
			{
				new URIish().GetHumanishName();
				NUnit.Framework.Assert.Fail("empty path is useless");
			}
			catch (ArgumentException)
			{
			}
		}

		// expected
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidWithEmptySlashDotGitHumanishName()
		{
			string humanishName = new URIish("/a/b/.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("b", humanishName);
		}

		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetWithSlashDotGitHumanishName()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, new URIish("/.git").GetHumanishName
				());
		}

		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetTwoSlashesDotGitHumanishName()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, new URIish("/.git").GetHumanishName
				());
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidHumanishName()
		{
			string humanishName = new URIish(GIT_SCHEME + "abc").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidSlashHumanishName()
		{
			string humanishName = new URIish(GIT_SCHEME + "abc/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetSlashValidSlashHumanishName()
		{
			string humanishName = new URIish("/abc/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetSlashValidSlashDotGitSlashHumanishName()
		{
			string humanishName = new URIish("/abc/.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetSlashSlashDotGitSlashHumanishName()
		{
			string humanishName = new URIish(GIT_SCHEME + "/abc//.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("may return an empty humanish name", string.Empty
				, humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetSlashesValidSlashHumanishName()
		{
			string humanishName = new URIish("/a/b/c/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("c", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidDotGitHumanishName()
		{
			string humanishName = new URIish(GIT_SCHEME + "abc.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidDotGitSlashHumanishName()
		{
			string humanishName = new URIish(GIT_SCHEME + "abc.git/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidWithSlashDotGitHumanishName()
		{
			string humanishName = new URIish("/abc.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidWithSlashDotGitSlashHumanishName()
		{
			string humanishName = new URIish("/abc.git/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("abc", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidWithSlashesDotGitHumanishName()
		{
			string humanishName = new URIish("/a/b/c.git").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("c", humanishName);
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual void TestGetValidWithSlashesDotGitSlashHumanishName()
		{
			string humanishName = new URIish("/a/b/c.git/").GetHumanishName();
			NUnit.Framework.Assert.AreEqual("c", humanishName);
		}
	}
}
