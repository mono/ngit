using System.Collections.Generic;
using NGit;
using NGit.Ignore;
using Sharpen;

namespace NGit.Ignore
{
	/// <summary>Represents a bundle of ignore rules inherited from a base directory.</summary>
	/// <remarks>
	/// Represents a bundle of ignore rules inherited from a base directory.
	/// This class is not thread safe, it maintains state about the last match.
	/// </remarks>
	public class IgnoreNode
	{
		/// <summary>
		/// Result from
		/// <see cref="IsIgnored(string, bool)">IsIgnored(string, bool)</see>
		/// .
		/// </summary>
		public enum MatchResult
		{
			NOT_IGNORED,
			IGNORED,
			CHECK_PARENT
		}

		/// <summary>The rules that have been parsed into this node.</summary>
		/// <remarks>The rules that have been parsed into this node.</remarks>
		private readonly IList<IgnoreRule> rules;

		/// <summary>Create an empty ignore node with no rules.</summary>
		/// <remarks>Create an empty ignore node with no rules.</remarks>
		public IgnoreNode()
		{
			rules = new AList<IgnoreRule>();
		}

		/// <summary>Create an ignore node with given rules.</summary>
		/// <remarks>Create an ignore node with given rules.</remarks>
		/// <param name="rules">list of rules.</param>
		public IgnoreNode(IList<IgnoreRule> rules)
		{
			this.rules = rules;
		}

		/// <summary>Parse files according to gitignore standards.</summary>
		/// <remarks>Parse files according to gitignore standards.</remarks>
		/// <param name="in">
		/// input stream holding the standard ignore format. The caller is
		/// responsible for closing the stream.
		/// </param>
		/// <exception cref="System.IO.IOException">Error thrown when reading an ignore file.
		/// 	</exception>
		public virtual void Parse(InputStream @in)
		{
			BufferedReader br = AsReader(@in);
			string txt;
			while ((txt = br.ReadLine()) != null)
			{
				txt = txt.Trim();
				if (txt.Length > 0 && !txt.StartsWith("#"))
				{
					rules.AddItem(new IgnoreRule(txt));
				}
			}
		}

		private static BufferedReader AsReader(InputStream @in)
		{
			return new BufferedReader(new InputStreamReader(@in, Constants.CHARSET));
		}

		/// <returns>list of all ignore rules held by this node.</returns>
		public virtual IList<IgnoreRule> GetRules()
		{
			return Sharpen.Collections.UnmodifiableList(rules);
		}

		/// <summary>Determine if an entry path matches an ignore rule.</summary>
		/// <remarks>Determine if an entry path matches an ignore rule.</remarks>
		/// <param name="entryPath">
		/// the path to test. The path must be relative to this ignore
		/// node's own repository path, and in repository path format
		/// (uses '/' and not '\').
		/// </param>
		/// <param name="isDirectory">true if the target item is a directory.</param>
		/// <returns>status of the path.</returns>
		public virtual IgnoreNode.MatchResult IsIgnored(string entryPath, bool isDirectory
			)
		{
			if (rules.IsEmpty())
			{
				return IgnoreNode.MatchResult.CHECK_PARENT;
			}
			// Parse rules in the reverse order that they were read
			for (int i = rules.Count - 1; i > -1; i--)
			{
				IgnoreRule rule = rules[i];
				if (rule.IsMatch(entryPath, isDirectory))
				{
					if (rule.GetResult())
					{
						return IgnoreNode.MatchResult.IGNORED;
					}
					else
					{
						return IgnoreNode.MatchResult.NOT_IGNORED;
					}
				}
			}
			return IgnoreNode.MatchResult.CHECK_PARENT;
		}
	}
}
