using System;
using NUnit.Framework;

namespace Sharpen.Test
{
	[TestFixture]
	public class FilePathTest
	{
		static bool RunningOnLinux = !Environment.OSVersion.Platform.ToString().StartsWith("Win");

		[Test]
		public void CombineTwoAbsolutes_Unix ()
		{
			var result = RunningOnLinux ? "/Foo/Bar" : @"C:\Foo\Bar";
			Assert.AreEqual(result, new FilePath("/Foo", "/Bar").GetAbsolutePath());
		}

		[Test]
		public void CombineTwoAbsolutes_DoubleSeperator_Unix ()
		{
			var result = RunningOnLinux ? "/Foo/Bar" : @"C:\Foo\Bar";
			Assert.AreEqual (result, new FilePath ("/Foo", "////Bar").GetAbsolutePath ());
		}

		[Test]
		public void CombineTwoAbsolutes_NullParentFilePath_Unix ()
		{
			var result = RunningOnLinux ? "/Bar" : @"C:\Bar";
			Assert.AreEqual(result, new FilePath((FilePath)null, "/Bar").GetAbsolutePath());
		}

		[Test]
		public void CombineTwoAbsolutes_NullParentString_Unix ()
		{
			var result = RunningOnLinux ? "/Bar" : @"C:\Bar";
			Assert.AreEqual(result, new FilePath((string)null, "/Bar").GetAbsolutePath());
		}

		[Test]
		public void CombineTwoAbsolutes_WindowsStyle_Unix ()
		{
			var result = RunningOnLinux ? @"/Foo/\Bar" : @"C:\Foo\Bar";
			Assert.AreEqual(result, new FilePath("/Foo", @"\Bar").GetAbsolutePath());
		}
	}
}
