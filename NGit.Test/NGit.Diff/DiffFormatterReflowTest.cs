using System;
using NGit.Diff;
using NGit.Patch;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class DiffFormatterReflowTest : TestCase
	{
		private RawText a;

		private RawText b;

		private FileHeader file;

		private ByteArrayOutputStream @out;

		private DiffFormatter fmt;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			@out = new ByteArrayOutputStream();
			fmt = new DiffFormatter(@out);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestNegativeContextFails()
		{
			Init("X");
			try
			{
				fmt.SetContext(-1);
				NUnit.Framework.Assert.Fail("accepted negative context");
			}
			catch (ArgumentException)
			{
			}
		}

		// pass
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext0()
		{
			Init("X");
			fmt.SetContext(0);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext1()
		{
			Init("X");
			fmt.SetContext(1);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext3()
		{
			Init("X");
			fmt.SetContext(3);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext5()
		{
			Init("X");
			fmt.SetContext(5);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext10()
		{
			Init("X");
			fmt.SetContext(10);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestContext100()
		{
			Init("X");
			fmt.SetContext(100);
			AssertFormatted();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestEmpty1()
		{
			Init("E");
			AssertFormatted("E.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestNoNewLine1()
		{
			Init("Y");
			AssertFormatted("Y.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestNoNewLine2()
		{
			Init("Z");
			AssertFormatted("Z.patch");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Init(string name)
		{
			a = new RawText(ReadFile(name + "_PreImage"));
			b = new RawText(ReadFile(name + "_PostImage"));
			file = ParseTestPatchFile(name + ".patch").GetFiles()[0];
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertFormatted()
		{
			AssertFormatted(Sharpen.Extensions.GetTestName(this) + ".out");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertFormatted(string name)
		{
			fmt.Format(file, a, b);
			string exp = RawParseUtils.Decode(ReadFile(name));
			NUnit.Framework.Assert.AreEqual(exp, RawParseUtils.Decode(@out.ToByteArray()));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] ReadFile(string patchFile)
		{
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				byte[] buf = new byte[1024];
				ByteArrayOutputStream temp = new ByteArrayOutputStream();
				int n;
				while ((n = @in.Read(buf)) > 0)
				{
					temp.Write(buf, 0, n);
				}
				return temp.ToByteArray();
			}
			finally
			{
				@in.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private NGit.Patch.Patch ParseTestPatchFile(string patchFile)
		{
			InputStream @in = GetType().GetResourceAsStream(patchFile);
			if (@in == null)
			{
				NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
				return null;
			}
			// Never happens
			try
			{
				NGit.Patch.Patch p = new NGit.Patch.Patch();
				p.Parse(@in);
				return p;
			}
			finally
			{
				@in.Close();
			}
		}
	}
}
