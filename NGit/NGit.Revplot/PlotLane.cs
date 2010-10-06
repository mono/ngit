using NGit.Revplot;
using Sharpen;

namespace NGit.Revplot
{
	/// <summary>A line space within the graph.</summary>
	/// <remarks>
	/// A line space within the graph.
	/// <p>
	/// Commits are strung onto a lane. For many UIs a lane represents a column.
	/// </remarks>
	public class PlotLane
	{
		internal int position;

		/// <summary>Logical location of this lane within the graphing plane.</summary>
		/// <remarks>Logical location of this lane within the graphing plane.</remarks>
		/// <returns>location of this lane, 0 through the maximum number of lanes.</returns>
		public virtual int GetPosition()
		{
			return position;
		}

		public override int GetHashCode()
		{
			return position;
		}

		public override bool Equals(object o)
		{
			return o == this;
		}
	}
}
