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
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class BlockListTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmptyList()
		{
			BlockList<string> empty;
			empty = new BlockList<string>();
			NUnit.Framework.Assert.AreEqual(0, empty.Count);
			NUnit.Framework.Assert.IsTrue(empty.IsEmpty());
			NUnit.Framework.Assert.IsFalse(empty.Iterator().HasNext());
			empty = new BlockList<string>(0);
			NUnit.Framework.Assert.AreEqual(0, empty.Count);
			NUnit.Framework.Assert.IsTrue(empty.IsEmpty());
			NUnit.Framework.Assert.IsFalse(empty.Iterator().HasNext());
			empty = new BlockList<string>(1);
			NUnit.Framework.Assert.AreEqual(0, empty.Count);
			NUnit.Framework.Assert.IsTrue(empty.IsEmpty());
			NUnit.Framework.Assert.IsFalse(empty.Iterator().HasNext());
			empty = new BlockList<string>(64);
			NUnit.Framework.Assert.AreEqual(0, empty.Count);
			NUnit.Framework.Assert.IsTrue(empty.IsEmpty());
			NUnit.Framework.Assert.IsFalse(empty.Iterator().HasNext());
		}

		[NUnit.Framework.Test]
		public virtual void TestGet()
		{
			BlockList<string> list = new BlockList<string>(4);
			string b;
			try
			{
				b = list[-1];
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual((-1).ToString(), badIndex.Message);
			}
			try
			{
				b = list[0];
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(0.ToString(), badIndex.Message);
			}
			try
			{
				b = list[4];
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(4.ToString(), badIndex.Message);
			}
			string fooStr = "foo";
			string barStr = "bar";
			string foobarStr = "foobar";
			list.AddItem(fooStr);
			list.AddItem(barStr);
			list.AddItem(foobarStr);
			NUnit.Framework.Assert.AreSame(fooStr, list[0]);
			NUnit.Framework.Assert.AreSame(barStr, list[1]);
			NUnit.Framework.Assert.AreSame(foobarStr, list[2]);
			try
			{
				b = list[3];
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(3.ToString(), badIndex.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestSet()
		{
			BlockList<string> list = new BlockList<string>(4);
			try
			{
				list.Set(-1, "foo");
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual((-1).ToString(), badIndex.Message);
			}
			try
			{
				list.Set(0, "foo");
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(0.ToString(), badIndex.Message);
			}
			try
			{
				list.Set(4, "foo");
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(4.ToString(), badIndex.Message);
			}
			string fooStr = "foo";
			string barStr = "bar";
			string foobarStr = "foobar";
			list.AddItem(fooStr);
			list.AddItem(barStr);
			list.AddItem(foobarStr);
			NUnit.Framework.Assert.AreSame(fooStr, list[0]);
			NUnit.Framework.Assert.AreSame(barStr, list[1]);
			NUnit.Framework.Assert.AreSame(foobarStr, list[2]);
			NUnit.Framework.Assert.AreSame(fooStr, list.Set(0, barStr));
			NUnit.Framework.Assert.AreSame(barStr, list.Set(1, fooStr));
			NUnit.Framework.Assert.AreSame(barStr, list[0]);
			NUnit.Framework.Assert.AreSame(fooStr, list[1]);
			try
			{
				list.Set(3, "bar");
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(3.ToString(), badIndex.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAddToEnd()
		{
			BlockList<int> list = new BlockList<int>(4);
			int cnt = BlockList<int>.BLOCK_SIZE * 3;
			for (int i = 0; i < cnt; i++)
			{
				list.AddItem(Sharpen.Extensions.ValueOf(42 + i));
			}
			NUnit.Framework.Assert.AreEqual(cnt, list.Count);
			for (int i_1 = 0; i_1 < cnt; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(42 + i_1), list[i_1]);
			}
			list.Clear();
			NUnit.Framework.Assert.AreEqual(0, list.Count);
			NUnit.Framework.Assert.IsTrue(list.IsEmpty());
			for (int i_2 = 0; i_2 < cnt; i_2++)
			{
				list.Add(i_2, Sharpen.Extensions.ValueOf(42 + i_2));
			}
			NUnit.Framework.Assert.AreEqual(cnt, list.Count);
			for (int i_3 = 0; i_3 < cnt; i_3++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(42 + i_3), list[i_3]);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestAddSlowPath()
		{
			BlockList<string> list = new BlockList<string>(4);
			string fooStr = "foo";
			string barStr = "bar";
			string foobarStr = "foobar";
			string firstStr = "first";
			string zeroStr = "zero";
			list.AddItem(fooStr);
			list.AddItem(barStr);
			list.AddItem(foobarStr);
			NUnit.Framework.Assert.AreEqual(3, list.Count);
			list.Add(1, firstStr);
			NUnit.Framework.Assert.AreEqual(4, list.Count);
			NUnit.Framework.Assert.AreSame(fooStr, list[0]);
			NUnit.Framework.Assert.AreSame(firstStr, list[1]);
			NUnit.Framework.Assert.AreSame(barStr, list[2]);
			NUnit.Framework.Assert.AreSame(foobarStr, list[3]);
			list.Add(0, zeroStr);
			NUnit.Framework.Assert.AreEqual(5, list.Count);
			NUnit.Framework.Assert.AreSame(zeroStr, list[0]);
			NUnit.Framework.Assert.AreSame(fooStr, list[1]);
			NUnit.Framework.Assert.AreSame(firstStr, list[2]);
			NUnit.Framework.Assert.AreSame(barStr, list[3]);
			NUnit.Framework.Assert.AreSame(foobarStr, list[4]);
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveFromEnd()
		{
			BlockList<string> list = new BlockList<string>(4);
			string fooStr = "foo";
			string barStr = "bar";
			string foobarStr = "foobar";
			list.AddItem(fooStr);
			list.AddItem(barStr);
			list.AddItem(foobarStr);
			NUnit.Framework.Assert.AreSame(foobarStr, list.Remove(2));
			NUnit.Framework.Assert.AreEqual(2, list.Count);
			NUnit.Framework.Assert.AreSame(barStr, list.Remove(1));
			NUnit.Framework.Assert.AreEqual(1, list.Count);
			NUnit.Framework.Assert.AreSame(fooStr, list.Remove(0));
			NUnit.Framework.Assert.AreEqual(0, list.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveSlowPath()
		{
			BlockList<string> list = new BlockList<string>(4);
			string fooStr = "foo";
			string barStr = "bar";
			string foobarStr = "foobar";
			list.AddItem(fooStr);
			list.AddItem(barStr);
			list.AddItem(foobarStr);
			NUnit.Framework.Assert.AreSame(barStr, list.Remove(1));
			NUnit.Framework.Assert.AreEqual(2, list.Count);
			NUnit.Framework.Assert.AreSame(fooStr, list[0]);
			NUnit.Framework.Assert.AreSame(foobarStr, list[1]);
			NUnit.Framework.Assert.AreSame(fooStr, list.Remove(0));
			NUnit.Framework.Assert.AreEqual(1, list.Count);
			NUnit.Framework.Assert.AreSame(foobarStr, list[0]);
			NUnit.Framework.Assert.AreSame(foobarStr, list.Remove(0));
			NUnit.Framework.Assert.AreEqual(0, list.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestAddRemoveAdd()
		{
			BlockList<int> list = new BlockList<int>();
			for (int i = 0; i < BlockList<int>.BLOCK_SIZE + 1; i++)
			{
				list.AddItem(Sharpen.Extensions.ValueOf(i));
			}
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(BlockList<int>.BLOCK_SIZE), 
				list.Remove(list.Count - 1));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(BlockList<int>.BLOCK_SIZE -
				 1), list.Remove(list.Count - 1));
			NUnit.Framework.Assert.IsTrue(list.AddItem(Sharpen.Extensions.ValueOf(1)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(1), list[list.Count - 
				1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestAddAllFromOtherList()
		{
			BlockList<int> src = new BlockList<int>(4);
			int cnt = BlockList<int>.BLOCK_SIZE * 2;
			for (int i = 0; i < cnt; i++)
			{
				src.AddItem(Sharpen.Extensions.ValueOf(42 + i));
			}
			src.AddItem(Sharpen.Extensions.ValueOf(1));
			BlockList<int> dst = new BlockList<int>(4);
			dst.AddItem(Sharpen.Extensions.ValueOf(255));
			dst.AddAll(src);
			NUnit.Framework.Assert.AreEqual(cnt + 2, dst.Count);
			for (int i_1 = 0; i_1 < cnt; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(42 + i_1), dst[i_1 + 1
					]);
			}
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(1), dst[dst.Count - 1]
				);
		}

		[NUnit.Framework.Test]
		public virtual void TestFastIterator()
		{
			BlockList<int> list = new BlockList<int>(4);
			int cnt = BlockList<int>.BLOCK_SIZE * 3;
			for (int i = 0; i < cnt; i++)
			{
				list.AddItem(Sharpen.Extensions.ValueOf(42 + i));
			}
			NUnit.Framework.Assert.AreEqual(cnt, list.Count);
			Iterator<int> itr = list.Iterator();
			for (int i_1 = 0; i_1 < cnt; i_1++)
			{
				NUnit.Framework.Assert.IsTrue(itr.HasNext());
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(42 + i_1), itr.Next());
			}
			NUnit.Framework.Assert.IsFalse(itr.HasNext());
		}

		[NUnit.Framework.Test]
		public virtual void TestAddRejectsBadIndexes()
		{
			BlockList<int> list = new BlockList<int>(4);
			list.AddItem(Sharpen.Extensions.ValueOf(41));
			try
			{
				list.Add(-1, Sharpen.Extensions.ValueOf(42));
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual((-1).ToString(), badIndex.Message);
			}
			try
			{
				list.Add(4, Sharpen.Extensions.ValueOf(42));
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(4.ToString(), badIndex.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestRemoveRejectsBadIndexes()
		{
			BlockList<int> list = new BlockList<int>(4);
			list.AddItem(Sharpen.Extensions.ValueOf(41));
			try
			{
				list.Remove(-1);
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual((-1).ToString(), badIndex.Message);
			}
			try
			{
				list.Remove(4);
			}
			catch (IndexOutOfRangeException badIndex)
			{
				NUnit.Framework.Assert.AreEqual(4.ToString(), badIndex.Message);
			}
		}
	}
}
