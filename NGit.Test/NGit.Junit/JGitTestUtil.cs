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
using System.Reflection;
using NGit.Junit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;
using System.IO;

namespace NGit.Junit
{
	public abstract class JGitTestUtil
	{
		public static readonly string CLASSPATH_TO_RESOURCES = "org/eclipse/jgit/test/resources/";

		public JGitTestUtil()
		{
			throw new NotSupportedException();
		}

/* Implemented in Sharpen.Extensions

		public static string GetName()
		{
			JGitTestUtil.GatherStackTrace stack;
			try
			{
				throw new JGitTestUtil.GatherStackTrace();
			}
			catch (JGitTestUtil.GatherStackTrace wanted)
			{
				stack = wanted;
			}
			try
			{
				foreach (StackTraceElement stackTrace in stack.GetStackTrace())
				{
					string className = stackTrace.GetClassName();
					string methodName = stackTrace.GetMethodName();
					MethodInfo method;
					try
					{
						method = Sharpen.Runtime.GetType(className).GetMethod(methodName, (Type[])null);
					}
					catch (NoSuchMethodException)
					{
						//
						// could be private, i.e. not a test method
						// could have arguments, not handled
						continue;
					}
					NUnit.Framework.Test annotation = method.GetAnnotation<NUnit.Framework.Test>();
					if (annotation != null)
					{
						return methodName;
					}
				}
			}
			catch (TypeLoadException)
			{
			}
			// Fall through and crash.
			throw new Exception("Cannot determine name of current test");
		}
		 */
		
		[System.Serializable]
		private class GatherStackTrace : Exception
		{
			// Thrown above to collect the stack frame.
		}

		public static void AssertEquals(byte[] exp, byte[] act)
		{
			NUnit.Framework.Assert.AreEqual(S(exp), S(act));
		}

		private static string S(byte[] raw)
		{
			return RawParseUtils.Decode(raw);
		}

		public static FilePath GetTestResourceFile(string fileName)
		{
			if (fileName == null || fileName.Length <= 0)
			{
				return null;
			}
			string path = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "resources");
			path = Path.Combine (path, "global");
			return new FilePath (Path.Combine (path, fileName));
		}
	}
}
