using System;
using NUnit.Framework;
using System.IO;

namespace Sharpen.Test
{
	[TestFixture]
	public class FilePathTest
	{
		static bool RunningOnLinux = !Environment.OSVersion.Platform.ToString().StartsWith("Win");

		[Test]
		public void SetLastWriteTime_Directory ()
		{
			var temp = (FilePath) Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
			Directory.CreateDirectory (temp);

			try {
				var lastWriteTime = temp.LastModified ();
				Assert.AreNotEqual (0, lastWriteTime, "#1");

				lastWriteTime += (long)TimeSpan.FromHours (1).TotalMilliseconds;
				temp.SetLastModified (lastWriteTime);
				Assert.AreEqual (lastWriteTime, temp.LastModified (), "#2");
			} finally {
				Directory.Delete (temp);
			}
		}

		[Test]
		public void SetLastWriteTime_File ()
		{
			var temp = (FilePath) Path.GetTempFileName ();
			try {
				var lastWriteTime = temp.LastModified ();
				Assert.AreNotEqual (0, lastWriteTime, "#1");
				
				lastWriteTime += (long)TimeSpan.FromHours (1).TotalMilliseconds;
				temp.SetLastModified (lastWriteTime);
				Assert.AreEqual (lastWriteTime, temp.LastModified (), "#2");
			} finally {
				File.Delete (temp);
			}
		}

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
