using System.IO;
using NGit;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	internal class FetchHeadRecord
	{
		internal ObjectId newValue;

		internal bool notForMerge;

		internal string sourceName;

		internal URIish sourceURI;

		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void Write(TextWriter pw)
		{
			string type;
			string name;
			if (sourceName.StartsWith(Constants.R_HEADS))
			{
				type = "branch";
				name = Sharpen.Runtime.Substring(sourceName, Constants.R_HEADS.Length);
			}
			else
			{
				if (sourceName.StartsWith(Constants.R_TAGS))
				{
					type = "tag";
					name = Sharpen.Runtime.Substring(sourceName, Constants.R_TAGS.Length);
				}
				else
				{
					if (sourceName.StartsWith(Constants.R_REMOTES))
					{
						type = "remote branch";
						name = Sharpen.Runtime.Substring(sourceName, Constants.R_REMOTES.Length);
					}
					else
					{
						type = string.Empty;
						name = sourceName;
					}
				}
			}
			pw.Write(newValue.Name);
			pw.Write('\t');
			if (notForMerge)
			{
				pw.Write("not-for-merge");
			}
			pw.Write('\t');
			pw.Write(type);
			pw.Write(" '");
			pw.Write(name);
			pw.Write("' of ");
			pw.Write(sourceURI.ToString());
			pw.Write("\n");
		}
	}
}
