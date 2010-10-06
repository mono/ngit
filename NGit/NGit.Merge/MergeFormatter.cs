using System.Collections.Generic;
using NGit.Diff;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>A class to convert merge results into a Git conformant textual presentation
	/// 	</summary>
	public class MergeFormatter
	{
		/// <summary>
		/// Formats the results of a merge of
		/// <see cref="NGit.Diff.RawText">NGit.Diff.RawText</see>
		/// objects in a Git
		/// conformant way. This method also assumes that the
		/// <see cref="NGit.Diff.RawText">NGit.Diff.RawText</see>
		/// objects
		/// being merged are line oriented files which use LF as delimiter. This
		/// method will also use LF to separate chunks and conflict metadata,
		/// therefore it fits only to texts that are LF-separated lines.
		/// </summary>
		/// <param name="out">the outputstream where to write the textual presentation</param>
		/// <param name="res">the merge result which should be presented</param>
		/// <param name="seqName">
		/// When a conflict is reported each conflicting range will get a
		/// name. This name is following the "<&lt;&lt;&lt;&lt;&lt;&lt; " or ">&gt;&gt;&gt;&gt;&gt;&gt; "
		/// conflict markers. The names for the sequences are given in
		/// this list
		/// </param>
		/// <param name="charsetName">
		/// the name of the characterSet used when writing conflict
		/// metadata
		/// </param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void FormatMerge(OutputStream @out, MergeResult<RawText> res, IList
			<string> seqName, string charsetName)
		{
			string lastConflictingName = null;
			// is set to non-null whenever we are
			// in a conflict
			bool threeWayMerge = (res.GetSequences().Count == 3);
			foreach (MergeChunk chunk in res)
			{
				RawText seq = res.GetSequences()[chunk.GetSequenceIndex()];
				if (lastConflictingName != null && chunk.GetConflictState() != MergeChunk.ConflictState
					.NEXT_CONFLICTING_RANGE)
				{
					// found the end of an conflict
					@out.Write(Sharpen.Runtime.GetBytesForString((">>>>>>> " + lastConflictingName + 
						"\n"), charsetName));
					lastConflictingName = null;
				}
				if (chunk.GetConflictState() == MergeChunk.ConflictState.FIRST_CONFLICTING_RANGE)
				{
					// found the start of an conflict
					@out.Write(Sharpen.Runtime.GetBytesForString(("<<<<<<< " + seqName[chunk.GetSequenceIndex
						()] + "\n"), charsetName));
					lastConflictingName = seqName[chunk.GetSequenceIndex()];
				}
				else
				{
					if (chunk.GetConflictState() == MergeChunk.ConflictState.NEXT_CONFLICTING_RANGE)
					{
						// found another conflicting chunk
						lastConflictingName = seqName[chunk.GetSequenceIndex()];
						@out.Write(Sharpen.Runtime.GetBytesForString((threeWayMerge ? "=======\n" : "======= "
							 + lastConflictingName + "\n"), charsetName));
					}
				}
				// the lines with conflict-metadata are written. Now write the chunk
				for (int i = chunk.GetBegin(); i < chunk.GetEnd(); i++)
				{
					seq.WriteLine(@out, i);
					@out.Write('\n');
				}
			}
			// one possible leftover: if the merge result ended with a conflict we
			// have to close the last conflict here
			if (lastConflictingName != null)
			{
				@out.Write(Sharpen.Runtime.GetBytesForString((">>>>>>> " + lastConflictingName + 
					"\n"), charsetName));
			}
		}

		/// <summary>
		/// Formats the results of a merge of exactly two
		/// <see cref="NGit.Diff.RawText">NGit.Diff.RawText</see>
		/// objects in
		/// a Git conformant way. This convenience method accepts the names for the
		/// three sequences (base and the two merged sequences) as explicit
		/// parameters and doesn't require the caller to specify a List
		/// </summary>
		/// <param name="out">
		/// the
		/// <see cref="Sharpen.OutputStream">Sharpen.OutputStream</see>
		/// where to write the textual
		/// presentation
		/// </param>
		/// <param name="res">the merge result which should be presented</param>
		/// <param name="baseName">the name ranges from the base should get</param>
		/// <param name="oursName">the name ranges from ours should get</param>
		/// <param name="theirsName">the name ranges from theirs should get</param>
		/// <param name="charsetName">
		/// the name of the characterSet used when writing conflict
		/// metadata
		/// </param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void FormatMerge(OutputStream @out, MergeResult<RawText> res, string baseName
			, string oursName, string theirsName, string charsetName)
		{
			IList<string> names = new AList<string>(3);
			names.AddItem(baseName);
			names.AddItem(oursName);
			names.AddItem(theirsName);
			FormatMerge(@out, res, names, charsetName);
		}
	}
}
