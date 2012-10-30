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

using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class PacketLineInTest
	{
		private ByteArrayInputStream rawIn;

		private PacketLineIn @in;

		// Note, test vectors created with:
		//
		// perl -e 'printf "%4.4x%s\n", 4+length($ARGV[0]),$ARGV[0]'
		// readString
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString1()
		{
			Init("0006a\n0007bc\n");
			NUnit.Framework.Assert.AreEqual("a", @in.ReadString());
			NUnit.Framework.Assert.AreEqual("bc", @in.ReadString());
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString2()
		{
			Init("0032want fcfcfb1fd94829c1a1704f894fc111d14770d34e\n");
			string act = @in.ReadString();
			NUnit.Framework.Assert.AreEqual("want fcfcfb1fd94829c1a1704f894fc111d14770d34e", 
				act);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString4()
		{
			Init("0005a0006bc");
			NUnit.Framework.Assert.AreEqual("a", @in.ReadString());
			NUnit.Framework.Assert.AreEqual("bc", @in.ReadString());
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString5()
		{
			// accept both upper and lower case
			Init("000Fhi i am a s");
			NUnit.Framework.Assert.AreEqual("hi i am a s", @in.ReadString());
			AssertEOF();
			Init("000fhi i am a s");
			NUnit.Framework.Assert.AreEqual("hi i am a s", @in.ReadString());
			AssertEOF();
		}

		[NUnit.Framework.Test]
		public virtual void TestReadString_LenHELO()
		{
			Init("HELO");
			try
			{
				@in.ReadString();
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: HELO", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadString_Len0001()
		{
			Init("0001");
			try
			{
				@in.ReadString();
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: 0001", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadString_Len0002()
		{
			Init("0002");
			try
			{
				@in.ReadString();
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: 0002", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadString_Len0003()
		{
			Init("0003");
			try
			{
				@in.ReadString();
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: 0003", e.Message);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString_Len0004()
		{
			Init("0004");
			string act = @in.ReadString();
			NUnit.Framework.Assert.AreEqual(string.Empty, act);
			NUnit.Framework.Assert.AreNotSame(PacketLineIn.END, act);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadString_End()
		{
			Init("0000");
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, @in.ReadString());
			AssertEOF();
		}

		// readStringNoLF
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadStringRaw1()
		{
			Init("0005a0006bc");
			NUnit.Framework.Assert.AreEqual("a", @in.ReadStringRaw());
			NUnit.Framework.Assert.AreEqual("bc", @in.ReadStringRaw());
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadStringRaw2()
		{
			Init("0031want fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			string act = @in.ReadStringRaw();
			NUnit.Framework.Assert.AreEqual("want fcfcfb1fd94829c1a1704f894fc111d14770d34e", 
				act);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadStringRaw3()
		{
			Init("0004");
			string act = @in.ReadStringRaw();
			NUnit.Framework.Assert.AreEqual(string.Empty, act);
			NUnit.Framework.Assert.AreNotSame(PacketLineIn.END, act);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadStringRaw_End()
		{
			Init("0000");
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, @in.ReadStringRaw());
			AssertEOF();
		}

		[NUnit.Framework.Test]
		public virtual void TestReadStringRaw4()
		{
			Init("HELO");
			try
			{
				@in.ReadStringRaw();
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: HELO", e.Message);
			}
		}

		// readACK
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_NAK()
		{
			ObjectId expid = ObjectId.FromString("fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			MutableObjectId actid = new MutableObjectId();
			actid.FromString(expid.Name);
			Init("0008NAK\n");
			NUnit.Framework.Assert.AreEqual(PacketLineIn.AckNackResult.NAK, @in.ReadACK(actid
				));
			NUnit.Framework.Assert.AreEqual(expid, actid);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_ACK1()
		{
			ObjectId expid = ObjectId.FromString("fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			MutableObjectId actid = new MutableObjectId();
			Init("0031ACK fcfcfb1fd94829c1a1704f894fc111d14770d34e\n");
			NUnit.Framework.Assert.AreEqual(PacketLineIn.AckNackResult.ACK, @in.ReadACK(actid
				));
			NUnit.Framework.Assert.AreEqual(expid, actid);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_ACKcontinue1()
		{
			ObjectId expid = ObjectId.FromString("fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			MutableObjectId actid = new MutableObjectId();
			Init("003aACK fcfcfb1fd94829c1a1704f894fc111d14770d34e continue\n");
			NUnit.Framework.Assert.AreEqual(PacketLineIn.AckNackResult.ACK_CONTINUE, @in.ReadACK
				(actid));
			NUnit.Framework.Assert.AreEqual(expid, actid);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_ACKcommon1()
		{
			ObjectId expid = ObjectId.FromString("fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			MutableObjectId actid = new MutableObjectId();
			Init("0038ACK fcfcfb1fd94829c1a1704f894fc111d14770d34e common\n");
			NUnit.Framework.Assert.AreEqual(PacketLineIn.AckNackResult.ACK_COMMON, @in.ReadACK
				(actid));
			NUnit.Framework.Assert.AreEqual(expid, actid);
			AssertEOF();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_ACKready1()
		{
			ObjectId expid = ObjectId.FromString("fcfcfb1fd94829c1a1704f894fc111d14770d34e");
			MutableObjectId actid = new MutableObjectId();
			Init("0037ACK fcfcfb1fd94829c1a1704f894fc111d14770d34e ready\n");
			NUnit.Framework.Assert.AreEqual(PacketLineIn.AckNackResult.ACK_READY, @in.ReadACK
				(actid));
			NUnit.Framework.Assert.AreEqual(expid, actid);
			AssertEOF();
		}

		[NUnit.Framework.Test]
		public virtual void TestReadACK_Invalid1()
		{
			Init("HELO");
			try
			{
				@in.ReadACK(new MutableObjectId());
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid packet header");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid packet line header: HELO", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadACK_Invalid2()
		{
			Init("0009HELO\n");
			try
			{
				@in.ReadACK(new MutableObjectId());
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid ACK/NAK");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Expected ACK/NAK, got: HELO", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadACK_Invalid3()
		{
			string s = "ACK fcfcfb1fd94829c1a1704f894fc111d14770d34e neverhappen";
			Init("003d" + s + "\n");
			try
			{
				@in.ReadACK(new MutableObjectId());
				NUnit.Framework.Assert.Fail("incorrectly accepted unsupported ACK status");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Expected ACK/NAK, got: " + s, e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestReadACK_Invalid4()
		{
			Init("0000");
			try
			{
				@in.ReadACK(new MutableObjectId());
				NUnit.Framework.Assert.Fail("incorrectly accepted no ACK/NAK");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual("Expected ACK/NAK, found EOF", e.Message);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadACK_ERR()
		{
			Init("001aERR want is not valid\n");
			try
			{
				@in.ReadACK(new MutableObjectId());
				NUnit.Framework.Assert.Fail("incorrectly accepted ERR");
			}
			catch (PackProtocolException e)
			{
				NUnit.Framework.Assert.AreEqual("want is not valid", e.Message);
			}
		}

		// test support
		private void Init(string msg)
		{
			rawIn = new ByteArrayInputStream(Constants.EncodeASCII(msg));
			@in = new PacketLineIn(rawIn);
		}

		private void AssertEOF()
		{
			NUnit.Framework.Assert.AreEqual(-1, rawIn.Read());
		}
	}
}
